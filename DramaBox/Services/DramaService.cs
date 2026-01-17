using DramaBox.Models;

namespace DramaBox.Services;

public class DramaService
{
    private readonly FirebaseDatabaseService _db;

    public DramaService(FirebaseDatabaseService db)
    {
        _db = db;
    }

    // -------------------------
    // OFFICIAL: Series
    // -------------------------
    public async Task<List<Drama>> GetOfficialSeriesAsync(CancellationToken ct = default)
    {
        // official/series -> { dramaId: {title..} }
        var map = await _db.GetAsync<Dictionary<string, Drama>>("official/series", ct)
                  ?? new Dictionary<string, Drama>();

        var list = new List<Drama>();

        foreach (var kv in map)
        {
            if (kv.Value == null) continue;
            kv.Value.Id = kv.Key;
            if (!kv.Value.Active) continue;
            list.Add(kv.Value);
        }

        return list
            .OrderBy(d => d.Order)
            .ThenByDescending(d => d.CreatedAt)
            .ToList();
    }

    // -------------------------
    // OFFICIAL: Episodes
    // -------------------------
    public async Task<List<Episode>> GetEpisodesAsync(string dramaId, string seasonId = "s1", CancellationToken ct = default)
    {
        dramaId = (dramaId ?? "").Trim();
        seasonId = (seasonId ?? "s1").Trim();
        if (string.IsNullOrWhiteSpace(dramaId)) return new List<Episode>();

        // official/episodes/{dramaId}/{seasonId} -> { e01: {..}, e02:{..} }
        var map = await _db.GetAsync<Dictionary<string, Episode>>($"official/episodes/{dramaId}/{seasonId}", ct)
                  ?? new Dictionary<string, Episode>();

        var list = new List<Episode>();
        foreach (var kv in map)
        {
            if (kv.Value == null) continue;
            kv.Value.Id = kv.Key;
            if (!kv.Value.Active) continue;
            list.Add(kv.Value);
        }

        return list
            .OrderBy(e => e.Order)
            .ThenBy(e => e.Number)
            .ToList();
    }

    // -------------------------
    // OFFICIAL: Home slots
    // -------------------------
    private class HomeSlot
    {
        public string DramaId { get; set; } = "";
        public string SeasonId { get; set; } = "s1";
        public string EpisodeId { get; set; } = "";
        public int Order { get; set; } = 0;
    }

    // DTO para o Hero do seu XAML
    public class HeroItemVm
    {
        public string DramaId { get; set; } = "";
        public string SeasonId { get; set; } = "s1";
        public string EpisodeId { get; set; } = "";

        public string CoverUrl { get; set; } = "";
        public string Kicker { get; set; } = "";
        public string Title { get; set; } = "";
        public string EpisodesText { get; set; } = "0 episódios";
        public string DurationText { get; set; } = "";
        public string AgeText { get; set; } = "";
    }

    // DTO para cards horizontais (New + Top10)
    public class CardItemVm
    {
        public string DramaId { get; set; } = "";

        public string CoverUrl { get; set; } = "";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";

        public bool HasTag { get; set; } = false;
        public string TagText { get; set; } = "";
        public string TagColor { get; set; } = "#3B82F6";

        public string Rank { get; set; } = ""; // Top10 grande
    }

    public class HomeData
    {
        public List<HeroItemVm> Hero { get; set; } = new();
        public List<CardItemVm> NewItems { get; set; } = new();
        public List<CardItemVm> Top10 { get; set; } = new();
    }

    public async Task<Drama?> GetDramaByIdAsync(string dramaId, CancellationToken ct = default)
    {
        dramaId = (dramaId ?? "").Trim();
        if (string.IsNullOrWhiteSpace(dramaId)) return null;

        // caminho direto
        var drama = await _db.GetAsync<Drama>($"official/series/{dramaId}", ct);
        if (drama != null)
        {
            drama.Id = dramaId;
            if (!drama.Active) return null;
            return drama;
        }

        // fallback: tenta pelo cache (lista)
        var list = await GetOfficialSeriesAsync(ct);
        return list.FirstOrDefault(d => string.Equals(d.Id, dramaId, StringComparison.OrdinalIgnoreCase));
    }


    public async Task<HomeData> GetOfficialHomeAsync(CancellationToken ct = default)
    {
        var result = new HomeData();

        // Carrega séries (pra resolver capa/titulo/etc)
        var series = await GetOfficialSeriesAsync(ct);
        var byId = series.ToDictionary(s => s.Id, s => s);

        // HERO slots
        var heroSlots = await _db.GetAsync<Dictionary<string, HomeSlot>>("official/home/hero", ct)
                       ?? new Dictionary<string, HomeSlot>();

        foreach (var slot in heroSlots.Values.Where(x => x != null).OrderBy(x => x.Order))
        {
            if (string.IsNullOrWhiteSpace(slot.DramaId)) continue;
            if (!byId.TryGetValue(slot.DramaId, out var drama)) continue;

            // episódios e duração: usa temporada/ep do slot, se existir
            var eps = await GetEpisodesAsync(slot.DramaId, string.IsNullOrWhiteSpace(slot.SeasonId) ? "s1" : slot.SeasonId, ct);
            var totalEps = eps.Count;

            string durationText = "";
            string seasonId = string.IsNullOrWhiteSpace(slot.SeasonId) ? "s1" : slot.SeasonId;
            string episodeId = slot.EpisodeId ?? "";

            if (!string.IsNullOrWhiteSpace(episodeId))
            {
                var ep = eps.FirstOrDefault(e => e.Id == episodeId);
                if (ep != null && ep.DurationSec > 0)
                {
                    // Ex: 420s -> 7 min
                    var min = Math.Max(1, (int)Math.Round(ep.DurationSec / 60.0));
                    durationText = $"{min} min";
                }
            }

            if (string.IsNullOrWhiteSpace(durationText))
                durationText = "2–5 min";

            result.Hero.Add(new HeroItemVm
            {
                DramaId = drama.Id,
                SeasonId = seasonId,
                EpisodeId = episodeId,
                CoverUrl = drama.CoverUrl,
                Kicker = $"{(string.IsNullOrWhiteSpace(drama.Kicker) ? "Destaque" : drama.Kicker)} • {drama.Genre}",
                Title = drama.Title,
                EpisodesText = $"{totalEps} episódios",
                DurationText = durationText,
                AgeText = string.IsNullOrWhiteSpace(drama.Age) ? "12+" : drama.Age
            });
        }

        // NEW slots
        var newSlots = await _db.GetAsync<Dictionary<string, HomeSlot>>("official/home/new", ct)
                      ?? new Dictionary<string, HomeSlot>();

        foreach (var slot in newSlots.Values.Where(x => x != null).OrderBy(x => x.Order))
        {
            if (string.IsNullOrWhiteSpace(slot.DramaId)) continue;
            if (!byId.TryGetValue(slot.DramaId, out var drama)) continue;

            var eps = await GetEpisodesAsync(drama.Id, "s1", ct);

            result.NewItems.Add(new CardItemVm
            {
                DramaId = drama.Id,
                CoverUrl = drama.CoverUrl,
                Title = drama.Title,
                Subtitle = $"{drama.Genre} • {eps.Count} eps",
                HasTag = true,
                TagText = "Novo",
                TagColor = "#2563EB"
            });
        }

        // TOP10 slots
        var topSlots = await _db.GetAsync<Dictionary<string, HomeSlot>>("official/home/top10", ct)
                      ?? new Dictionary<string, HomeSlot>();

        var orderedTop = topSlots.Values.Where(x => x != null).OrderBy(x => x.Order).ToList();

        for (int i = 0; i < orderedTop.Count; i++)
        {
            var slot = orderedTop[i];
            if (string.IsNullOrWhiteSpace(slot.DramaId)) continue;
            if (!byId.TryGetValue(slot.DramaId, out var drama)) continue;

            result.Top10.Add(new CardItemVm
            {
                DramaId = drama.Id,
                CoverUrl = drama.CoverUrl,
                Title = drama.Title,
                Subtitle = drama.Genre,
                Rank = (i + 1).ToString()
            });
        }

        // fallback se não tiver nada (pra não quebrar UI)
        if (result.Hero.Count == 0 && series.Count > 0)
        {
            var d = series[0];
            var eps = await GetEpisodesAsync(d.Id, "s1", ct);

            result.Hero.Add(new HeroItemVm
            {
                DramaId = d.Id,
                SeasonId = "s1",
                EpisodeId = eps.FirstOrDefault()?.Id ?? "",
                CoverUrl = d.CoverUrl,
                Kicker = $"Destaque • {d.Genre}",
                Title = d.Title,
                EpisodesText = $"{eps.Count} episódios",
                DurationText = "2–5 min",
                AgeText = string.IsNullOrWhiteSpace(d.Age) ? "12+" : d.Age
            });
        }

        return result;
    }
}
