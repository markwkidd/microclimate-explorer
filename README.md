# How local is your local weather data?

## Purpose

This application will find the four closest Citizen Weather Observer Program stations to any longitude and latitude. Put another way, Microclimate Explorer is a utility for accessing and utilizing geographically-granular climate data.

Whereas there are many solutions for accessing aggregated and averaged weather data offered at high resolution, it is more difficult to access non-aggregated data from weather sensors. 

Uses are myriad, but the inspiration for this application comes from agriculture and land use planning.

## Data sources

The most important dataset is citizen weather stations that are part of the CWOP network and provide data to Meterological Assimilation Data Ingest System (MADIS).

### Using FindU API

**Please note:** Microclimate Explorer v0.1 relies on the FindU database and API for geographical search of CWOP stations. During the course of development, the author communicated with NOAA, which hosts FindU because of trouble using some functionality. NOAA staff indicated that FindU was remaining online, but was no longer being maintained and some parts have failed.

None of the several other weather station APIs offer geographical search functionality, but the raw public dataset includes coordinates and it is reaonable that another solution could be developed.

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
- Write a text JSON file to save a configuration setting
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

