using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace Microclimate_Explorer
{
    public class LocationService
    {
        public async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    return (0, 0);
                }

                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                });

                return location != null
                    ? (location.Latitude, location.Longitude)
                    : (0, 0);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }

        // Method to manually enter location
        public (double Latitude, double Longitude) GetLocationByCoordinates(string latitudeStr, string longitudeStr)
        {
            if (double.TryParse(latitudeStr, out double latitude) &&
                double.TryParse(longitudeStr, out double longitude))
            {
                // Optional: Add basic validation for coordinate ranges
                if (latitude >= -90 && latitude <= 90 &&
                    longitude >= -180 && longitude <= 180)
                {
                    return (latitude, longitude);
                }
            }

            return (0, 0);
        }
    }
}