using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microclimate_Explorer
{
    public class GeocodingService
    {
        private const string BaseUrl = "https://api.opencagedata.com/geocode/v1/json";

        public async Task<(double Latitude, double Longitude)> GeocodeLocationAsync(string locationName, string apiKey)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"{BaseUrl}?q={Uri.EscapeDataString(locationName)}&key={apiKey}";
                var response = await httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var results = json.RootElement.GetProperty("results");
                if (results.GetArrayLength() > 0)
                {
                    var firstResult = results[0];
                    var geometry = firstResult.GetProperty("geometry");
                    var latitude = geometry.GetProperty("lat").GetDouble();
                    var longitude = geometry.GetProperty("lng").GetDouble();
                    return (latitude, longitude);
                }
            }

            return (0, 0);
        }
    }
}
