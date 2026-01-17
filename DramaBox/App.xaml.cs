using DramaBox.Views;

namespace DramaBox;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new SplashPage())
        {
            BarBackgroundColor = (Color)Current.Resources["Bg"],
            BarTextColor = Colors.White
        };

#if WINDOWS
        ApplyWindowsPhoneLikeWindow();
#endif
    }

#if WINDOWS
    private static void ApplyWindowsPhoneLikeWindow()
    {
        // Garante que a janela já exista antes de configurar
        Microsoft.Maui.Controls.Application.Current!.Dispatcher.Dispatch(() =>
        {
            var app = Microsoft.Maui.Controls.Application.Current;
            if (app?.Windows == null || app.Windows.Count == 0) return;

            var win = app.Windows[0];
            if (win == null) return;

            // ===== Config padrão "simular celular" =====
            // Altura máxima solicitada
            const double maxH = 800;

            // Aspect ratio de celular (largura/altura)
            // 9:16 => 0.5625
            const double ratioWbyH = 9.0 / 16.0;

            // Largura proporcional à altura máxima
            var targetH = maxH;
            var targetW = Math.Round(targetH * ratioWbyH);

            // Limites mínimos (para não ficar pequeno demais)
            win.MinimumHeight = 640;
            win.MinimumWidth = 360;

            // Limites máximos
            win.MaximumHeight = maxH;
            win.MaximumWidth = Math.Max(targetW, win.MinimumWidth);

            // Tamanho inicial
            win.Height = targetH;
            win.Width = Math.Max(targetW, win.MinimumWidth);

            // Centraliza (opcional) mantendo consistente
            win.X = (win.X < 0) ? 0 : win.X;
            win.Y = (win.Y < 0) ? 0 : win.Y;
        });
    }
#endif
}
