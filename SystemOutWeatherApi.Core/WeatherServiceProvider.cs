using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SystemOut.Toolbox.Core;
using SystemOut.WeatherApi.Core.Models.OpenWeatherMap;
using SystemOutWeatherApi.Core;
using Newtonsoft.Json;

namespace SystemOut.WeatherApi.Core
{
    internal class WeatherServiceProvider : IWeatherServiceProvider
    {
        private readonly IHttpClient httpClient;
        public WeatherServiceProvider()
        {
            httpClient = new HttpClientImp();
        }

        public WeatherServiceProvider(IHttpClient mock)
        {
            httpClient = mock;
        }

        public async Task<WeatherData> ExecuteAsync(string uri)
        {
            try
            {
                var json = await httpClient.GetStringAsync(uri);

                Rootobject weatherData;
                try
                {
                    weatherData = JsonConvert.DeserializeObject<Rootobject>(json);
                }
                catch (JsonSerializationException jsonException)
                {
                    throw new WeatherServiceException("Unable to parse the weather data into a known object type.", jsonException);
                }

                var weatherDetails = weatherData.weather.FirstOrDefault();
                if (weatherDetails?.main == null)
                {
                    throw new WeatherServiceException("An unexpected weather data string was returned: " + json);
                }

                var weatherString = weatherDetails.description;
                if (string.IsNullOrEmpty(weatherString))
                    weatherString = weatherDetails.main;

                return new WeatherData
                {
                    Description = weatherString.FirstCharToUpper(),
                    Location = weatherData.name,
                    Temp = weatherData.main.temp,
                    WeatherIconUri = new Uri($"http://openweathermap.org/img/w/{weatherDetails.icon}.png")
                };
            }
            catch (HttpRequestException httpException)
            {
                throw new WeatherServiceException("An http error occurred, probably due to a network error.", httpException);

            }
        }

        class HttpClientImp : IHttpClient
        {
            public async Task<string> GetStringAsync(string uri)
            {
                var webClient = new HttpClient();
                return await webClient.GetStringAsync(uri);
            }
        }
    }
}
