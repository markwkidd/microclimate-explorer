namespace Microclimate_Explorer
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationService _locationService;

        public MainPage(LocationService locationService)
        {
            InitializeComponent();
            _locationService = locationService;
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