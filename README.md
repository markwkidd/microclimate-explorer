# How local is your local weather data?

## Purpose

This application will find the four closest Citizen Weather Observer Program stations to any longitude and latitude. Put another way, Microclimate Explorer is a utility for accessing and utilizing geographically-granular climate data.

Uses include precise data for agricultural planning and other situations where it is valuable to be able to directly access weather station data for a designated area.

## Data sources

The most important dataset is citizen weather stations. This data is aggregated by the APRS-IS (Automatic Packet Reporting System-Internet Service).

Microclimate Explorer make use of this data ultimately aggregated by the Meterological Assimilation Data Ingest System (MADIS). This data was generally processed by the findU database of weather, position, telemetry, and message data.

## API Keys

Version 0.1 of Microclimate Explorer requires an OpenCage API key in order to convert place names and addresses into latitude and longitude, although that is not the only way to enter coordinates into this software. A key is available from the author, and you can get a free API key at [OpenCage](https://opencagedata.com/).

# Implementation

## Technology and frameworks

- C# MAUI Blazor
- Modular code with object-oriented data structures
- OpenCage geocoding API
- FindU weather station data web API scraper (scraper used with permission)

### Software development features

- Use of a list data structure
- Package sample FindU data with the .NET solution that can be parsed from local file storage
- TODO: Write a text log file
- TODO: Regular expression (regex) for input validation
- TODO: Code comments relecting use of SOLID principles
- TODO: Use of a new generic class

## Building this project

On the developer's Visual Studio workstation, it was necessary to install Selenium via NuGet.

### .Net Packages for Windows 10

```
   Top-level Package                         Requested   Resolved
   > Microsoft.Extensions.Logging.Debug      9.0.0       9.0.0   
   > Microsoft.Maui.Controls                 9.0.14      9.0.14  
   > Selenium.WebDriver                      4.30.0      4.30.0  
   > System.Net.Http.Json                    7.0.1       7.0.1   
```

