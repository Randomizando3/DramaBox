using CommunityToolkit.Maui;
using DramaBox.Services;
using DramaBox.ViewModels;
using DramaBox.Views;
using Microsoft.Extensions.Logging;

namespace DramaBox;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // -----------------------------
        // SERVICES (Firebase + domínio)
        // -----------------------------
        // Auth estava faltando: LoginPage/RegisterPage precisam dele
        builder.Services.AddSingleton<FirebaseAuthService>();

        // Database + DramaService
        builder.Services.AddSingleton<FirebaseDatabaseService>();
        builder.Services.AddSingleton<DramaService>();

        // -----------------------------
        // VIEWMODELS
        // -----------------------------
        // Como suas Pages resolvem via ServiceProvider no OnAppearing,
        // Singleton funciona bem e evita re-instanciar a cada navegação.
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<DiscoverViewModel>();

        // Se você tiver outros VMs na estrutura, pode registrar também:
        // builder.Services.AddSingleton<LoginViewModel>();
        // builder.Services.AddSingleton<RegisterViewModel>();
        // builder.Services.AddSingleton<SearchViewModel>();
        // builder.Services.AddSingleton<ProfileViewModel>();

        // -----------------------------
        // VIEWS / PAGES
        // -----------------------------
        // Obs: o XAML do TabbedPage cria as páginas por construtor padrão,
        // então elas precisam existir com construtor público sem parâmetros.
        // Registrar aqui não é obrigatório para o TabbedPage,
        // mas ajuda para navegação via DI depois.
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<DiscoverPage>();
        builder.Services.AddTransient<LibraryPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DramaDetailsPage>();
        builder.Services.AddTransient<PlayerPage>();
        builder.Services.AddTransient<MainTabsPage>();

        return builder.Build();
    }
}
