using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

public class LocationService
{
    private readonly IGeolocation _geolocation;
    private readonly HttpClient _httpClient;

    public LocationService(IGeolocation geolocation, HttpClient httpClient)
    {
        _geolocation = geolocation;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Attempts to get the device's current location
    /// </summary>
    /// <returns>Location coordinates or null if unable to retrieve</returns>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            // Check location permissions
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    return null;
            }

            // Get location with high accuracy and a reasonable timeout
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
            var location = await _geolocation.GetLocationAsync(request);
            return location;
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            Console.WriteLine($"Error getting location: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Geocodes a location string to GPS coordinates
    /// </summary>
    /// <param name="locationQuery">City, State, Zip, or Address</param>
    /// <returns>Location coordinates or null if unable to geocode</returns>
    public async Task<Location?> GeocodeLocationAsync(string locationQuery)
    {
        // Use OpenStreetMap Nominatim for free geocoding
        string encodedQuery = Uri.EscapeDataString(locationQuery);
        string url = $"https://nominatim.openstreetmap.org/search?format=json&q={encodedQuery}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<NominatimResult>>(content);

                if (locations != null && locations.Count > 0)
                {
                    return new Location(
                        Convert.ToDouble(locations[0].Latitude, CultureInfo.InvariantCulture),
                        Convert.ToDouble(locations[0].Longitude, CultureInfo.InvariantCulture)
                    );
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Geocoding error: {ex.Message}");
            return null;
        }
    }

    // Helper class for Nominatim JSON deserialization
    private class NominatimResult
    {
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}

// Example usage in a Blazor component
public partial class LocationComponent
{
    private string _locationInput = string.Empty;
    private Location? _currentLocation;
    private Location? _searchedLocation;

    private async Task GetCurrentLocation()
    {
        _currentLocation = await LocationService.GetCurrentLocationAsync();
    }

    private async Task SearchLocation()
    {
        _searchedLocation = await LocationService.GeocodeLocationAsync(_locationInput);
    }
}