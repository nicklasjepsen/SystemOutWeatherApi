using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemOut.WeatherApi.Core.Models.OpenWeatherMap;

namespace SystemOutWeatherApi.Core
{
    public interface IWeatherServiceProvider
    {
        Task<WeatherData> ExecuteAsync(string uri);
    }
}
