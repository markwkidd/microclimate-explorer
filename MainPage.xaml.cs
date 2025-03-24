using Microsoft.Maui.Devices.Sensors;

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckAndRequestLocationPermission();
        }

        private async Task CheckAndRequestLocationPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    // Optional: You can retrieve location immediately after permission is granted
                    var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                    // TODO: Update UI or perform actions with location
                    // For example:
                    // LocationLabel.Text = $"Lat: {latitude}, Lon: {longitude}";
                }
                else
                {
                    // Handle permission denied
                    await DisplayAlert("Permission Denied",
                        "Location access is required for this app to function properly.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // Optional: Add a button to manually trigger location retrieval
        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            await CheckAndRequestLocationPermission();
        }
    }
}