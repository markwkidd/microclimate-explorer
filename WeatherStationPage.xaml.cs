using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace Microclimate_Explorer
{
    public partial class WeatherStationPage : ContentPage
    {
        private readonly WebScrapingService _webScrapingService;
        public ObservableCollection<WeatherStation> WeatherStations { get; set; } = new ObservableCollection<WeatherStation>();

        public WeatherStationPage(double latitude, double longitude, WebScrapingService webScrapingService)
        {
            InitializeComponent();
            _webScrapingService = webScrapingService;
            CoordinatesLabel.Text = $"Latitude: {latitude}, Longitude: {longitude}";
            BindingContext = this; // Ensure the BindingContext is set to the current instance
        }

        private async void OnLoadSampleDataClicked(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                // Get the sample file path
                string sampleFilePath = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Examples", "eastern-ky-weather-stations.htm");

                // For testing purposes, we'll use a file URI to load the sample data
                // In a real app, you would ensure the file exists or extract it from resources
                if (!File.Exists(sampleFilePath))
                {
                    // For testing, copy the file from app resources to app data directory
                    using var stream = await FileSystem.OpenAppPackageFileAsync("Resources/Examples/eastern-ky-weather-stations.htm");
                    if (stream != null)
                    {
                        var directory = Path.GetDirectoryName(sampleFilePath);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        using var fileStream = File.Create(sampleFilePath);
                        await stream.CopyToAsync(fileStream);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Sample data file not found in resources.", "OK");
                        return;
                    }
                }

                // Load weather data from the local file
                var weatherStations = await _webScrapingService.LoadWeatherDataFromFileAsync(sampleFilePath);

                UpdateWeatherStationsList(weatherStations);

                await DisplayAlert("Success", $"Loaded {weatherStations.Count} weather stations from sample data.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load sample data: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnScrapeFindUClicked(object sender, EventArgs e)
        {
            try
            {
                var url = WeatherUrlEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(url))
                {
                    await DisplayAlert("Error", "Please enter a URL to scrape", "OK");
                    return;
                }

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var weatherStations = await _webScrapingService.ScrapeWeatherStationsAsync(url);

                UpdateWeatherStationsList(weatherStations);

                await DisplayAlert("Success", $"Scraped {weatherStations.Count} weather stations from the web.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to scrape web data: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private void UpdateWeatherStationsList(List<WeatherStation> weatherStations)
        {
            WeatherStations.Clear();

            foreach (var station in weatherStations)
            {
                WeatherStations.Add(station);
            }
        }
    }
}
