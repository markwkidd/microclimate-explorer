# How local is your local weather data?

## Purpose

This application will find the four closest Citizen Weather Observer Program stations to any longitude and latitude. Put another way, Microclimate Explorer is a utility for accessing and utilizing geographically-granular climate data.

Uses include precise data for agricultural planning and other situations where it is valuable to be able to directly access weather station data for a designated area.

## Data sources

The most important dataset is citizen weather stations. This data is aggregated by the APRS-IS (Automatic Packet Reporting System-Internet Service).

Microclimate Explorer make use of this data ultimately aggregated by the Meterological Assimilation Data Ingest System (MADIS). This data was generally processed by the findU database of weather, position, telemetry, and message data.

## Implementation

### Technology and frameworks

- C# MAUI Blazor
- Modular code with object-oriented data structures
- OpenCage geocoding API
- FindU weather station data web API scraper (scraper used with permission)
- Sample FindU data distributed with the solution and scraped from local file storage


### Software development features

- TODO: Regular expression (regex) for input validation
- Use of a list data structure
- TODO: Write a text log file
- Code comments relecting use of SOLID principles
- Use of a new generic class

## Building and running this project

Placeholder. Instructions to run the project.
