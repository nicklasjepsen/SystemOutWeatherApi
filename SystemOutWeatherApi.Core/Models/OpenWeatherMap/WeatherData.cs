using System;

namespace SystemOut.WeatherApi.Core.Models.OpenWeatherMap
{
    public class WeatherData
    {
        public float Temp { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Uri WeatherIconUri { get; set; }
    }
}