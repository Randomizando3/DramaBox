using DramaBox.Models;

namespace DramaBox.Views;

public partial class PlayerPage : ContentPage
{
    public PlayerPage(Drama drama, Episode ep)
    {
        InitializeComponent();

        DramaLbl.Text = drama.Title;
        EpLbl.Text = $"Ep {ep.Number} • {ep.Title}";

        LoadVideo(ep.VideoUrl);
    }

    private void LoadVideo(string? url)
    {
        url ??= "";
        url = url.Trim();

        // HTML simples com video tag
        // playsinline ajuda no iOS; controls + autoplay + muted melhora compatibilidade de autoplay
        var html = $@"
<!doctype html>
<html>
<head>
  <meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1' />
  <style>
    html, body {{
      margin:0; padding:0; background:#000; height:100%; width:100%;
      overflow:hidden;
    }}
    .wrap {{
      height:100%; width:100%;
      display:flex; align-items:center; justify-content:center;
      background:#000;
    }}
    video {{
      width:100%;
      height:100%;
      object-fit:contain;
      background:#000;
    }}
  </style>
</head>
<body>
  <div class='wrap'>
    <video controls autoplay muted playsinline webkit-playsinline>
      <source src='{EscapeHtmlAttr(url)}' />
      Seu dispositivo não suporta reprodução de vídeo.
    </video>
  </div>
</body>
</html>";

        VideoWeb.Source = new HtmlWebViewSource { Html = html };
    }

    private static string EscapeHtmlAttr(string s)
    {
        // escape básico para atributo HTML
        return (s ?? "")
            .Replace("&", "&amp;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }
}
