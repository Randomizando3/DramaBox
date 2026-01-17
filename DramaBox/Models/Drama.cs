namespace DramaBox.Models;

public class Drama
{
    public string Id { get; set; } = "";

    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public string Synopsis { get; set; } = "";

    public string CoverUrl { get; set; } = "";
    public string Kicker { get; set; } = "";
    public string Age { get; set; } = "";

    public bool Active { get; set; } = true;
    public int Order { get; set; } = 0;

    public long CreatedAt { get; set; } = 0;

    // ---- NOVO: usado em telas como Biblioteca/Autorais para mostrar "X eps"
    public int EpisodesCount { get; set; } = 0;
}
