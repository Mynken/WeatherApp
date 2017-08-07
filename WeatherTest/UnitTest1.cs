using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Weather;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace WeatherTest
{
    [TestClass]
    public class UnitTest1
    {
        static class Data
        {
            public static string Value { get; set; }
        }
        [TestMethod]
        public void SetTextToForm()
        {
            TextBox textbox1 = new TextBox();
            textbox1.Text = "Rzeszow";
            Data.Value = textbox1.Text.ToString();
            Assert.AreEqual(textbox1.Text, Data.Value);
        }

        [TestMethod]
        public async Task SetLabelAsync()
        {
            string city = "Rzeszow";
            using (Form1 mainform = new Form1())
            {
                OpenWeather forecast = JsonConvert.DeserializeObject<OpenWeather>(await GetWeatherForCityAsync(city));
                Label l1 = new Label();
                Label l2 = new Label();
                Label l3 = new Label();
                Label l4 = new Label();
                Label l5 = new Label();
                Label l6 = new Label();
                Label l7 = new Label();
                Label l8 = new Label();
                Label l9 = new Label();
                l1.Text = forecast.weather[0].main;
                l2.Text = forecast.weather[0].description;
                l3.Text = forecast.main.temp.ToString();
                l4.Text = forecast.main.humidity.ToString();
                l5.Text = forecast.main.pressure.ToString();
                l6.Text = forecast.wind.speed.ToString();
                l7.Text = forecast.wind.deg.ToString();
                l8.Text = forecast.name.ToString();
                l9.Text = forecast.sys.country.ToString();

                Assert.AreEqual(forecast.weather[0].main, l1);
                Assert.AreEqual(forecast.weather[0].description, l2);
                Assert.AreEqual(forecast.main.temp, l3);
                Assert.AreEqual(forecast.main.humidity, l4);
                Assert.AreEqual(forecast.main.pressure, l5);
                Assert.AreEqual(forecast.wind.speed, l6);
                Assert.AreEqual(forecast.wind.deg, l7);
                Assert.AreEqual(forecast.name, l8);
                Assert.AreEqual(forecast.sys.country, l9);
            }
            // OpenWeather forecast = new OpenWeather();

            //forecast.weather[0].main = "rain";
            //forecast.weather[0].description = "hard rain";
            //forecast.main.temp = 20;

            //forecast.main.humidity = 23;
            //forecast.main.pressure = 700;
            //forecast.wind.speed = 25;
            //forecast.wind.deg = 50;
            //forecast.name = "Rzeszow";
            //forecast.sys.country = "PL";


            //   mainForm.GetLabels(forecast, DateTime.Now, DateTime.UtcNow);
           



          
        }

        private async Task<string> GetWeatherForCityAsync(string city)
        {
            WebRequest request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?q={city}&APPID=133f53a4ef2155e4c0e5054102d7b8b1");
            request.Method = "POST";
            request.ContentType = "application/x-www-urlencoded";

            WebResponse response = await request.GetResponseAsync();
            string answer = String.Empty;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    answer = await reader.ReadToEndAsync();
                }
            }

            response.Close();
            return answer;
        }
    }
}
