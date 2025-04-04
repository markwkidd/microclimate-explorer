using Microsoft.Maui.Controls;

namespace Microclimate_Explorer
{
    public partial class WeatherStationPage : ContentPage
    {
        public WeatherStationPage(double latitude, double longitude)
        {
            InitializeComponent();
            CoordinatesLabel.Text = $"Latitude: {latitude}, Longitude: {longitude}";

            // Use the latitude and longitude to fetch and display weather data
        }
    }
}