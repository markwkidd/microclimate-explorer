﻿using System.Collections.ObjectModel;

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
            return Navigation.PushAsync(new WeatherStationPage(_lastLatitude, _lastLongitude));
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            if (LocationService.ValidateCoordinates(_lastLatitude, _lastLongitude))
            {
                await NavigateToWeatherStationPage();
            }
            else
            {
                await DisplayAlert("Error", "Please enter valid latitude and longitude values.", "OK");
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
