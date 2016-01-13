using System;
using System.Threading.Tasks;
using SystemOut.WeatherApi.Core.Models.OpenWeatherMap;
using SystemOutWeatherApi.Core;

namespace SystemOut.WeatherApi.Core
{
    /// <summary>
    /// This class uses the OpenWeatherMap to get weather data.
    /// See: http://openweathermap.org/current
    /// </summary>

    public class WeatherService
    {
        private readonly IWeatherServiceProvider weatherServiceProvider;
        public string AppId { get; }
        public string Uri => $"http://api.openweathermap.org/data/2.5/weather?APPID={AppId}&units=metric&";

        public WeatherService(string appId)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentException(nameof(appId));
            AppId = appId;
            weatherServiceProvider = new WeatherServiceProvider();
        }

        public WeatherService(IHttpClient mock)
        {
            AppId = "InvalidAppId";
            weatherServiceProvider = new WeatherServiceProvider(mock);
        }

        public async Task<WeatherData> GetWeatherDataForCoordinates(string lon, string lat)
        {
            return await weatherServiceProvider.ExecuteAsync($"{Uri}lat={lat}&lon={lon}");
        }
        public async Task<WeatherData> GetWeatherDataForCity(string cityName)
        {
            return await weatherServiceProvider.ExecuteAsync($"{Uri}q={cityName}");
        }

        public async Task<WeatherData> GetWeatherDataForCity(string zip, string country)
        {
            return await weatherServiceProvider.ExecuteAsync($"{Uri}zip={zip},{country}");
        }

        public async Task<WeatherData> GetWeatherDataForCityId(string cityId)
        {
            return await weatherServiceProvider.ExecuteAsync($"{Uri}id={cityId}");
        }
    }
}
