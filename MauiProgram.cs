using Microsoft.Extensions.Logging;

namespace Microclimate_Explorer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            builder.Services.AddSingleton(Geolocation.Default);
            builder.Services.AddSingleton<LocationService>();
            builder.Services.AddSingleton<GeocodingService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}