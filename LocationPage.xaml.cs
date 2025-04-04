using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Microclimate_Explorer
{
    public partial class LocationPage : ContentPage
    {
        private readonly LocationService _locationService;
        private readonly GeocodingService _geocodingService;
        private readonly WebScrapingService _webScrapingService;

        public ObservableCollection<WeatherStation> WeatherStations { get; set; } = new ObservableCollection<WeatherStation>();

        private double _lastLatitude;
        private double _lastLongitude;

        public LocationPage(LocationService locationService, GeocodingService geocodingService, WebScrapingService webScrapingService)
        {
            InitializeComponent();
            _locationService = locationService;
            _geocodingService = geocodingService;
            _webScrapingService = webScrapingService;

            CheckForApiKeyFile();

            BindingContext = this;
        }

        private void CheckForApiKeyFile()
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "microclimate-explorer-lowsecurity.txt");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var apiKeyData = JsonSerializer.Deserialize<ApiKeyData>(json);
                if (apiKeyData != null)
                {
                    ApiKeyEntry.Text = apiKeyData.OpenCageApiKey;
                }
            }
        }

        private async void OnSaveApiKeyClicked(object sender, EventArgs e)
        {
            var apiKey = ApiKeyEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await DisplayAlert("Error", "Please enter an OpenCage API key", "OK");
                return;
            }

            var filePath = Path.Combine(FileSystem.AppDataDirectory, "microclimate-explorer-lowsecurity.txt");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var apiKeyData = new ApiKeyData { OpenCageApiKey = apiKey };
            var json = JsonSerializer.Serialize(apiKeyData);
            File.WriteAllText(filePath, json);

            await DisplayAlert("Success", "OpenCage API key saved successfully", "OK");
        }

        private async void OnDeleteApiKeyClicked(object sender, EventArgs e)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "microclimate-explorer-lowsecurity.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    await DisplayAlert("Success", "OpenCage API key file deleted successfully", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while deleting the OpenCage API key file: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "OpenCage API key file does not exist", "OK");
            }
        }

        private class ApiKeyData
        {
            public string OpenCageApiKey { get; set; }
        }

        private Task NavigateToWeatherStationPage()
        {
            return Navigation.PushAsync(new WeatherStationPage(_lastLatitude, _lastLongitude, _webScrapingService));
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await NavigateToWeatherStationPage();
        }

        private async void OnManualLocationClicked(object sender, EventArgs e)
        {
            var (latitude, longitude) = _locationService.GetLocationByCoordinates(
                LatitudeEntry.Text,
                LongitudeEntry.Text
            );

            if (latitude == -999 || longitude == -999)
            {
                NextButton.IsEnabled = false;
                await DisplayAlert("Error", "Invalid coordinates have been entered.", "OK");
                return;
            }
            else
            {
                _lastLatitude = latitude;
                _lastLongitude = longitude;
                LocationResultLabel.Text = $"{latitude}, {longitude}";
                NextButton.IsEnabled = true;
            }
        }

        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                if (latitude == -999 && longitude == -999)
                {
                    await DisplayAlert("Location Error",
                        "Could not retrieve current location. Please enter coordinates manually.",
                        "OK");
                    NextButton.IsEnabled = false;
                }
                else
                {
                    _lastLatitude = latitude;
                    _lastLongitude = longitude;
                    LocationResultLabel.Text = $"{latitude}, {longitude}";
                    NextButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                NextButton.IsEnabled = false;
            }
        }

        private async void OnGeocodeLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var locationName = LocationNameEntry.Text?.Trim();
                var apiKey = ApiKeyEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(locationName))
                {
                    NextButton.IsEnabled = false;
                    await DisplayAlert("Error", "Please enter a location name", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    NextButton.IsEnabled = false;
                    await DisplayAlert("Error", "Please enter an OpenCage API key", "OK");
                    return;
                }

                var (latitude, longitude) = await _geocodingService.GeocodeLocationAsync(locationName, apiKey);

                if (latitude == 0 && longitude == 0)
                {
                    await DisplayAlert("Geocoding Error",
                        "Could not find coordinates for the given location. Please try a different location.",
                        "OK");
                    NextButton.IsEnabled = false;
                }
                else
                {
                    _lastLatitude = latitude;
                    _lastLongitude = longitude;
                    LocationResultLabel.Text = $"{latitude}, {longitude}";
                    NextButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                NextButton.IsEnabled = false;
            }
        }

    }
}
