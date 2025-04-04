namespace Microclimate_Explorer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            DisableWeatherStationPage();
        }

        public void EnableWeatherStationPage()
        {
            var weatherStationFlyoutItem = this.Items.FirstOrDefault(item => item.Title == "Weather Stations");
            if (weatherStationFlyoutItem != null)
            {
                weatherStationFlyoutItem.IsEnabled = true;
            }
        }

        private void DisableWeatherStationPage()
        {
            var weatherStationFlyoutItem = this.Items.FirstOrDefault(item => item.Title == "Weather Stations");
            if (weatherStationFlyoutItem != null)
            {
                weatherStationFlyoutItem.IsEnabled = false;
            }
        }
    }
}
