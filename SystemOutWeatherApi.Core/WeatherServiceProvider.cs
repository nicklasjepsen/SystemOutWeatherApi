using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SystemOut.Toolbox;
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
                var jsonObject = JsonConvert.DeserializeObject<Rootobject>(json);
                var weatherString = jsonObject.weather.First().description;
                if (string.IsNullOrEmpty(weatherString))
                    weatherString = jsonObject.weather.First().main;

                return new WeatherData
                {
                    Description = weatherString.FirstCharToUpper(),
                    Location = jsonObject.name,
                    Temp = jsonObject.main.temp,
                    WeatherIconUri = new Uri($"http://openweathermap.org/img/w/{jsonObject.weather.First().icon}.png")
                };
            }
            catch (HttpRequestException)
            {
                return null;
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
