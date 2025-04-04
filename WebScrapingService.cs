using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microclimate_Explorer
{
    public class WeatherStation
    {
        public string CallSign { get; set; }
        public double Distance { get; set; }
        public string Direction { get; set; }
        public string ReportAge { get; set; }
        public double? Temperature { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindGust { get; set; }
        public string WindDirection { get; set; }
        public double? RainLastHour { get; set; }
        public double? Rain24Hours { get; set; }
        public double? RainSinceMidnight { get; set; }
        public double? Humidity { get; set; }
        public double? Barometer { get; set; }
        public double Latitude { get; set; } = -999;
        public double Longitude { get; set; } = -999;
    }

    public class WebScrapingService
    {
        private readonly ChromeOptions _chromeOptions;

        public WebScrapingService()
        {
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument("--headless");
            _chromeOptions.AddArgument("--disable-gpu");
            _chromeOptions.AddArgument("--no-sandbox");
            _chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            _chromeOptions.AddArgument("--disable-css");
        }

        public async Task<List<WeatherStation>> ScrapeWeatherStationsAsync(string url)
        {
            return await Task.Run(() => ScrapeWeatherStations(url));
        }

        public List<WeatherStation> ScrapeWeatherStations(string url)
        {
            var weatherStations = new List<WeatherStation>();

            using (var driver = new ChromeDriver(_chromeOptions))
            {
                try
                {
                    driver.Navigate().GoToUrl(url);

                    // Wait for table to load
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElements(By.TagName("table")).Count > 0);

                    // Find all table rows but skip the header row
                    var rows = driver.FindElements(By.TagName("tr")).Skip(1).ToList();

                    foreach (var row in rows)
                    {
                        try
                        {
                            var cells = row.FindElements(By.TagName("td")).ToList();

                            if (cells.Count < 13)
                                continue;

                            // Extract the callsign from the anchor tag
                            var callSignElement = cells[0].FindElement(By.TagName("a"));
                            var callSign = callSignElement.Text.Trim();

                            var station = new WeatherStation
                            {
                                CallSign = callSign,
                                Distance = ParseDoubleOrNull(cells[1].Text) ?? 0,
                                Direction = cells[2].Text.Trim(),
                                ReportAge = cells[3].Text.Trim(),
                                Temperature = ParseDoubleOrNull(cells[4].Text),
                                WindSpeed = ParseDoubleOrNull(cells[5].Text),
                                WindGust = ParseDoubleOrNull(cells[6].Text),
                                WindDirection = cells[7].Text.Trim(),
                                RainLastHour = ParseDoubleOrNull(cells[8].Text),
                                Rain24Hours = ParseDoubleOrNull(cells[9].Text),
                                RainSinceMidnight = ParseDoubleOrNull(cells[10].Text),
                                Humidity = ParseDoubleOrNull(cells[11].Text),
                                Barometer = ParseDoubleOrNull(cells[12].Text)
                            };

                            weatherStations.Add(station);
                        }
                        catch (Exception ex)
                        {
                            // Skip problematic rows but log the error
                            Console.WriteLine($"Error parsing row: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping weather data: {ex.Message}");
                }
                finally
                {
                    driver.Quit();
                }
            }

            return weatherStations;
        }

        // Helper method to parse values that might be empty or non-numeric
        private double? ParseDoubleOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (double.TryParse(value, out double result))
                return result;

            return null;
        }

        // load weather data from local HTML file
        public async Task<List<WeatherStation>> LoadWeatherDataFromFileAsync(string filePath)
        {
            return await Task.Run(() => {
                var fileUrl = new Uri(filePath).AbsoluteUri;
                return ScrapeWeatherStations(fileUrl);
            });
        }

    }
}