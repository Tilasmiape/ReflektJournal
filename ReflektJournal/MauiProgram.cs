
using Microsoft.Extensions.Logging;
using ReflektJournal.Data;
using ReflektJournal.Services;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        SQLitePCL.Batteries_V2.Init();

        builder.Services.AddSingleton<ReflektDatabase>();

        builder.Services.AddSingleton<IToastService, ToastService>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IJournalService, JournalService>();
        builder.Services.AddSingleton<IMoodService, MoodService>();
        builder.Services.AddSingleton<ITagService, TagService>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();
        builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
        builder.Services.AddSingleton<IPdfExportService, PdfExportService>();

        return builder.Build();
    }
}
