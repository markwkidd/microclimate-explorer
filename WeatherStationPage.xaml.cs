using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace Microclimate_Explorer
{
    public partial class WeatherStationPage : ContentPage
    {
        private readonly WebScrapingService _webScrapingService;
        private string _weatherStationUrl;
        public ObservableCollection<WeatherStation> WeatherStations { get; set; } = new ObservableCollection<WeatherStation>();

        public WeatherStationPage(double latitude, double longitude, WebScrapingService webScrapingService)
        {
            InitializeComponent();
            _webScrapingService = webScrapingService;
            CoordinatesLabel.Text = $"{latitude}, {longitude}";
            _weatherStationUrl = $"http://www.findu.com/cgi-bin/wxnear.cgi?lat={latitude}&lon={longitude}";
            BindingContext = this;
        }

        public WeatherStationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void OnLoadSampleDataClicked(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                string sampleFilePath = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Examples", "eastern-ky-weather-stations.htm");

                var weatherStations = await _webScrapingService.LoadWeatherDataFromFileAsync(sampleFilePath);

                FindNearestStations(weatherStations);

                await DisplayAlert("Success", $"Loaded weather stations from local example data.", "OK");
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
                var url = _weatherStationUrl?.Trim();

                if (string.IsNullOrWhiteSpace(url))
                {
                    await DisplayAlert("Error", "The findU request URL has not been generated yet", "OK");
                    return;
                }

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var weatherStations = await _webScrapingService.ScrapeWeatherStationsAsync(url);

                FindNearestStations(weatherStations);

                await DisplayAlert("Success", $"Scraped {weatherStations.Count} nearby weather stations from the web.", "OK");
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

        private async void FindNearestStations(List<WeatherStation> weatherStations)
        {
            WeatherStations.Clear();

            var closestStations = weatherStations.OrderBy(ws => ws.Distance).Take(4).ToList();

            foreach (var station in closestStations)
            {
                WeatherStations.Add(station);
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
