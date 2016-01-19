using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SystemOut.Toolbox.Core;
using SystemOut.WeatherApi.Core.Models.OpenWeatherMap;
using SystemOutWeatherApi.Core;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.UI.Core;
using Newtonsoft.Json;

namespace SystemOut.WeatherApi.Core
{
    internal class WeatherServiceProvider : IWeatherServiceProvider
    {
        private readonly ResourceLoader resourceLoader;
        private readonly IHttpClient httpClient;
        private readonly CultureInfo localization;
        public WeatherServiceProvider(CultureInfo localization)
        {
            httpClient = new HttpClientImp();
            this.localization = localization;
            resourceLoader = ResourceLoader.GetForCurrentView();
        }

        public WeatherServiceProvider(IHttpClient mock, CultureInfo localization)
        {
            httpClient = mock;
            this.localization = localization;
            resourceLoader = ResourceLoader.GetForCurrentView();
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

                var weatherDetails = weatherData?.weather?.FirstOrDefault();
                if (weatherDetails?.main == null)
                {
                    throw new WeatherServiceException("An unexpected weather data string was returned: " + json);
                }

                var weatherString = weatherDetails.description;
                if (weatherDetails.id == 800)
                {
                    //var resL = new ResourceLoader("Resources");
                    //var resMan = ResourceManager.Current;
                    //ResourceMap libmap = ResourceManager.Current.AllResourceMaps["SystemOutWeatherApi.Core"];
                    //string lib = libmap.GetSubtree("Resources").GetValue(weatherDetails.id + string.Empty).ToString();
                    //ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();
                    if (localization.TwoLetterISOLanguageName == new CultureInfo("da-DK").TwoLetterISOLanguageName)
                    {
                        //weatherString = ResourceLoader.GetStringForReference(new Uri("ms-resource://SystemOutWeatherApi.Core/Resources/200"));
                        //weatherString = libmap.GetSubtree("Resources").GetValue(weatherDetails.id + string.Empty).ToString();
                        
                            weatherString =
                                resourceLoader.GetString(weatherDetails.id + string.Empty);
                        
                    }
                }

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
