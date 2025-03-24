namespace Microclimate_Explorer
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationService _locationService;
        private readonly GeocodingService _geocodingService;

        public MainPage(LocationService locationService, GeocodingService geocodingService)
        {
            InitializeComponent();
            _locationService = locationService;
            _geocodingService = geocodingService;
        }

        private async void OnGeocodeLocationClicked(object sender, EventArgs e)
        {
            try
            {
                var locationName = LocationNameEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(locationName))
                {
                    await DisplayAlert("Error", "Please enter a location name", "OK");
                    return;
                }

                var (latitude, longitude) = await _geocodingService.GeocodeLocationAsync(locationName);

                if (latitude == 0 && longitude == 0)
                {
                    await DisplayAlert("Geocoding Error",
                        "Could not find coordinates for the given location. Please try a different location.",
                        "OK");
                }
                else
                {
                    LocationResultLabel.Text = $"Geocoded Location: {latitude}, {longitude}";

                    // Optionally, pre-fill the latitude and longitude entries
                    LatitudeEntry.Text = latitude.ToString();
                    LongitudeEntry.Text = longitude.ToString();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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
                }
                else
                {
                    LocationResultLabel.Text = $"Location: {latitude}, {longitude}";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
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
            }
            else
            {
                LocationResultLabel.Text = $"Manual Location: {latitude}, {longitude}";
            }
        }
    }
}