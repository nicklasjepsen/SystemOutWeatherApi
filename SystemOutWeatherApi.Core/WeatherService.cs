using System;
using System.Globalization;
using System.Threading.Tasks;
using SystemOut.WeatherApi.Core.Models.OpenWeatherMap;
using SystemOutWeatherApi.Core;
using Windows.Globalization;
using Windows.UI.Xaml;

namespace SystemOut.WeatherApi.Core
{
    /// <summary>
    /// This class uses the OpenWeatherMap to get weather data.
    /// See: http://openweathermap.org/current
    /// </summary>

    public class WeatherService
    {
        public CultureInfo Localization { get; }

        private readonly IWeatherServiceProvider weatherServiceProvider;
        public string AppId { get; }
        public string Uri => $"http://api.openweathermap.org/data/2.5/weather?APPID={AppId}&units=metric&";

        public WeatherService(string appId)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentException(nameof(appId));
            AppId = appId;
            Localization = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride);
            weatherServiceProvider = new WeatherServiceProvider(Localization);
        }

        public WeatherService(string appId, CultureInfo localization)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentException(nameof(appId));
            AppId = appId;
            Localization = localization;
            weatherServiceProvider = new WeatherServiceProvider(Localization);
        }

        public WeatherService(IHttpClient mock)
        {
            AppId = "InvalidAppId";
            Localization = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride);
            weatherServiceProvider = new WeatherServiceProvider(mock, Localization);
        }

        public WeatherService(IHttpClient mock, CultureInfo localization)
        {
            AppId = "InvalidAppId";
            Localization = localization;
            weatherServiceProvider = new WeatherServiceProvider(mock, Localization);
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
