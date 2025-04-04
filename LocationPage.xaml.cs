using System.Collections.ObjectModel;

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

            BindingContext = this;
        }

        private Task NavigateToWeatherStationPage()
        {
            return Navigation.PushAsync(new WeatherStationPage(_lastLatitude, _lastLongitude, _webScrapingService));
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await NavigateToWeatherStationPage();
        }

        private void OnManualLocationClicked(object sender, EventArgs e)
        {
            var (latitude, longitude) = _locationService.GetLocationByCoordinates(
                LatitudeEntry.Text,
                LongitudeEntry.Text
            );

            if (latitude == 0 && longitude == 0)
            {
                LocationResultLabel.Text = "Invalid coordinates. Please enter valid numbers.";
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

        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                if (latitude == 0 && longitude == 0)
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
                    await DisplayAlert("Error", "Please enter a location name", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(apiKey))
                {
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
                    LatitudeEntry.Text = latitude.ToString();
                    LongitudeEntry.Text = longitude.ToString();
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
