using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SystemOut.WeatherApi.Core;
using Windows.Globalization;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestGetDescriptionDefaultLanguage()
        {
            var provider = new WeatherService(new TestMock("Assets/TestData/WeatherId800.json"));
            var result = await provider.GetWeatherDataForCity("copenhagen");
            Assert.IsNotNull(result);
            Assert.AreEqual("Sky is Clear", result.Description);
        }

        [TestMethod]
        public async Task TestGetDescriptionDaDkLanguage()
        {
            var provider = new WeatherService(new TestMock("Assets/TestData/WeatherId800.json"), new CultureInfo("da-DK"));
            var result = await provider.GetWeatherDataForCity("copenhagen");
            Assert.IsNotNull(result);
            Assert.AreEqual("skyfrit", result.Description);
        }
    }

    class TestMock : IHttpClient
    {
        private readonly string returnVal;
        public Task<string> GetStringAsync(string uri)
        {
            return Task.Run(() => returnVal);
        }

        public TestMock(string testDataPath)
        {
            returnVal = File.ReadAllText(testDataPath);
        }
    }
}
