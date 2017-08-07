using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using Weather.Entities;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Reflection;
using Weather.FiveDaysWeather;

namespace Weather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            #region Start bindings
           
            pictureBox1.SendToBack();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Dock = DockStyle.Fill;

            List<Label> list = new List<Label>() { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13 };
            foreach (var item in list)
            {
                item.Parent = pictureBox1;
                item.BackColor = Color.Transparent;
            }
            labelsControl1.Parent = pictureBox1;
            labelsControl1.BackColor = Color.Transparent;
            checkBox1.Parent = pictureBox1;
            checkBox1.BackColor = Color.Transparent;
            #endregion
        }
        private bool SetAutoRunValue(bool autorun, string path)
        {
            const string name = "WeatherApp";
            string exePath = path;
            RegistryKey reg;
            try
            {
                reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");

                if (autorun)
                {
                    reg.SetValue(name, exePath);
                }
                else
                {
                    reg.DeleteValue(name);
                }

                reg.Flush();
                reg.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public async Task<string> GetWeatherForCityAsync(string city)
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
        
        public async Task<string> GetSunRiseSetTime(string suntime)
        {
            var requstForSun = WebRequest.Create($"http://www.convert-unix-time.com/api?timestamp={suntime}");

            WebResponse responseSun = await requstForSun.GetResponseAsync();
            string answerSunrise = String.Empty;
            using (Stream s = responseSun.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    answerSunrise = await reader.ReadToEndAsync();
                }
            }

            responseSun.Close();
            return answerSunrise;
        }
        
        public async Task<DateTime> GetInfoAsync(string time)
        {
            Sun time1 = JsonConvert.DeserializeObject<Sun>(await GetSunRiseSetTime(time));
            string[] sun = time1.localDate.Split(' ');
            string sunVal = sun[4] + sun[5]; ;
            DateTime sungo = DateTime.Parse(sunVal);
            return sungo;
        }
        
        public void SetLabels(Color color)
        {
            List<Label> list = new List<Label>() { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13 };
            foreach (var item in list)
            {
                item.ForeColor = color;
            }
            labelsControl1.ForeColor = color;
        }
        
        public void GetBackGroundImage(string image)
        {
                switch (image.ToLower())
            {
                case string s when ((s.Contains("thunderstorm") || s.Contains("storm") || s.Contains("gale")) && (!s.Contains("rain") && !s.Contains("drizzle"))):
                    pictureBox1.Image = Properties.Resources.thunderstrom;
                    SetLabels(Color.Transparent);
                        break;

                case string s when ((s.Contains("drizzle") || s.Contains("rain")) && (!s.Contains("snow"))):
                    pictureBox1.Image = Properties.Resources.rain;
                    SetLabels(Color.Transparent);
                    break;

                case string s when s.Contains("snow"):
                    pictureBox1.Image = Properties.Resources.snow;
                    SetLabels(Color.Navy);
                    break;

                case "mist":
                case "smoke":
                case "haze":
                case "sand, dust whirls":
                case "fog":
                case "sand":
                case "dust":
                case "volcanic ash":
                case "squalls":
                    pictureBox1.Image = Properties.Resources.atmosphere;
                    SetLabels(Color.OrangeRed);
                    break;

                case "clear":
                        pictureBox1.Image = Properties.Resources.clear;
                        SetLabels(Color.Transparent);
                    break;

                case string s when s.Contains("clouds"):
                    pictureBox1.Image = Properties.Resources.clouds;
                    SetLabels(Color.Black);
                    break;

                case "tornado":
                case "hot":
                case "hurricane":
                case "cold":
                case "windy":
                case "hail":
                    pictureBox1.Image = Properties.Resources.tornado;
                    SetLabels(Color.ForestGreen);
                    break;

                case string s when s.Contains("breeze"):
                    pictureBox1.Image = Properties.Resources.breeze;
                    SetLabels(Color.Transparent);
                    break;

                default:
                    pictureBox1.Image = Properties.Resources._default;
                    break;
            }
        }
        
        //public void LoadToDb()
        //{
        //    try
        //    {
        //        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Weather;Integrated Security=True; MultipleActiveResultSets=True";

        //        string sqlInsert = $"INSERT INTO Weather (City, Description, Temperature, Status, Wspeed, Wdirection, Wet, Pressure, Sunrise, Sunset)" +
        //                $" VALUES ('{label8.Text.ToString()}', '{label2.Text.ToString()}', '{label3.Text.ToString()}', '{label1.Text.ToString()}', " +
        //                $"'{label6.Text.ToString()}', '{label7.Text.ToString()}', '{label4.Text.ToString()}', " +
        //                $"'{label5.Text.ToString()}', '{label11.Text.ToString()}', '{label12.Text.ToString()}')";

        //        string sqlRead = "SELECT * FROM Weather WHERE Id = (SELECT MAX(Id) FROM Weather)";

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            //   connection.Open();
        //            SqlCommand command = new SqlCommand(sqlRead, connection);
        //            SqlDataReader reader = command.ExecuteReader();
        //            if (reader.HasRows) // if data exsist
        //            {
        //                reader.Read();
        //                if (label8.Text.ToString() == reader["City"].ToString() && label3.Text.ToString() == reader["Temperature"].ToString() && label1.Text.ToString() == reader["Status"].ToString())
        //                {
        //                    reader.Close();
        //                }
        //                else
        //                {
        //                    SqlCommand command2 = new SqlCommand(sqlInsert, connection);
        //                    command2.ExecuteNonQuery();
        //                }
        //                reader.Close();
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        MessageBox.Show(ex.ToString());
        //    }
            
        //}
        
        //public void ReadFromDb()
        //{
        //    string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Weather;Integrated Security=True";
        //    string sqlExpression = "SELECT * FROM Weather WHERE Id = (SELECT MAX(Id) FROM Weather)";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        SqlCommand command = new SqlCommand(sqlExpression, connection);
        //        SqlDataReader reader = command.ExecuteReader();
        //        if (reader.HasRows) // if data exsist
        //        {
        //            reader.Read();
        //            label8.Text = reader["City"].ToString();
        //            label2.Text = reader["Description"].ToString();
        //            label3.Text = reader["Temperature"].ToString();
        //            label1.Text = reader["Status"].ToString();
        //            label6.Text = reader["Wspeed"].ToString();
        //            label7.Text = reader["Wdirection"].ToString();
        //            label4.Text = reader["Wet"].ToString();
        //            label5.Text = reader["Pressure"].ToString();
        //            label11.Text = reader["Sunrise"].ToString();
        //            label12.Text = reader["Sunset"].ToString();
        //        }
        //        GetBackGroundImage(label1.Text);
        //        reader.Close();
        //    }
        //}
        
        public string GetLabels(OpenWeather Ow, DateTime sunrise, DateTime sunset)
        {
            label1.Text = Ow.weather[0].main;
            label2.Text = Ow.weather[0].description;

            if (!Ow.main.temp.ToString().Contains("-"))
            {
                label3.Text = Ow.main.temp.ToString("+ 0.## °C");
            }
            else
            {
                label3.Text = Ow.main.temp.ToString(" 0.## °C");
                pictureBox1.Image = Properties.Resources.winter_day;

            }

            label4.Text = "Humidity: " + Ow.main.humidity.ToString() + "%";
            label5.Text = "Pressure: " + ((int)Ow.main.pressure).ToString() + " mm";
            label6.Text = "Speed (m/s): " + Ow.wind.speed.ToString();
            #region Direction of the wind
            switch (Ow.wind.deg)
            {
                #region North - East
                case double s when ((s >= 0) && (s <= 20) || (s > 340)):
                    label7.Text = "North  " + s + "°";
                    break;

                case double s when ((s > 20) && (s <= 45)):
                    label7.Text = "North - East  " + s + "°";
                    break;

                case double s when ((s > 45) && (s <= 70)):
                    label7.Text = "East - North  " + s + "°";
                    break;

                case double s when ((s > 70) && (s <= 110)):
                    label7.Text = "East  " + s + "°";
                    break;
                #endregion
                #region East - South - West
                case double s when ((s > 110 ) && (s <= 135)):
                    label7.Text = "East - South  " + s + "°"; ;
                    break;

                case double s when ((s > 135) && (s <= 160)):
                    label7.Text = "South - East  " + s + "°"; ;
                    break;

                case double s when ((s > 160) && (s <= 200)):
                    label7.Text = "South  " + s + "°"; ;
                    break;

                case double s when ((s > 200) && (s <= 225)):
                    label7.Text = "South - West  " + s + "°"; ;
                    break;

                case double s when ((s > 225) && (s <= 250)):
                    label7.Text = "West - South  " + s + "°"; ;
                    break;
                #endregion
                #region West - North
                case double s when ((s > 250) && (s <= 290)):
                    label7.Text = "West  " + s + "°";
                    break;
                case double s when ((s > 290) && (s <= 315)):
                    label7.Text = "West - North  " + s + "°";
                    break;
                case double s when ((s > 315) && (s <= 340)):
                    label7.Text = "North - West  " + s + "°";
                    break;
#endregion
                default:
                    label7.Text = "Direction  " + Ow.wind.deg.ToString() + "°";
                    break;
            }
            #endregion
            label8.Text = " " + Ow.name.ToString() + " (" + Ow.sys.country.ToString() + ")";

            label11.Text = "Sunrise at  " + sunrise.ToString("HH:mm");
           //   label12.Text = "Sunset at  " + sunset.ToString("HH:mm");

            labelsControl1.Sunrise = "Sunrise at  " + sunrise.ToString("HH:mm");
            labelsControl1.Sunset = "Sunset at  " + sunset.ToString("HH:mm");
            return null;
        }


        public string GetLocation()
        {
            var locationResponse = new WebClient().DownloadString("https://freegeoip.net/xml/");

            var responseXml = XDocument.Parse(locationResponse.ToString())
                .Element("Response");

            string city = responseXml.Element("City").Value.ToString();
            return city;
        }


        private async void Form1_Load(object sender, EventArgs e)
        {    
            try
            {
                labelsControl1.Sunrise = "align";
                labelsControl1.Sunset = "qwewar0";
                string city = GetLocation();
                try
                {
                    OpenWeather Ow = JsonConvert.DeserializeObject<OpenWeather>(await GetWeatherForCityAsync(city));

                    #region GetTimeForSun

                    DateTime sunrisetime = await GetInfoAsync(Ow.sys.sunrise.ToString());
                    DateTime sunsettime = await GetInfoAsync(Ow.sys.sunset.ToString());

                    #endregion
                    
                    GetBackGroundImage(Ow.weather[0].main);
                    GetLabels(Ow, sunrisetime, sunsettime);
                   // LoadToDb();
                }
                catch (Exception)
                {
                    MessageBox.Show("No city founded");
                    textBox1.Clear();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No Intrenet connection, data will be loaded from last online time");
                textBox1.Clear();
                try
                {
                  //  ReadFromDb();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No data was cached");
                    textBox1.Clear();
                }
            }           
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string city = textBox1.Text.ToString();           
            try
            {
                OpenWeather Ow = JsonConvert.DeserializeObject<OpenWeather>(await GetWeatherForCityAsync(city));

                #region GetTimeForSun

                DateTime sunrisetime = await GetInfoAsync(Ow.sys.sunrise.ToString());
                DateTime sunsettime = await GetInfoAsync(Ow.sys.sunset.ToString());

                #endregion

                GetBackGroundImage(Ow.weather[0].main);
                GetLabels(Ow, sunrisetime, sunsettime);
            }
            catch (Exception)
            {
                try
                {
                    WebClient Client = new WebClient();
                    String Response = Client.DownloadString("http://www.google.com");
                    MessageBox.Show("No city founded");
                    textBox1.Clear();
                }
                catch (Exception)
                {
                    MessageBox.Show("No internet connection");
                    textBox1.Clear();
                }
            }          
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    SetAutoRunValue(true, Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    SetAutoRunValue(false, Assembly.GetExecutingAssembly().Location);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                Data.Value = textBox1.Text.ToString();
                FiveDays weather = new FiveDays();
                weather.Show();
            }
            else
            {
                Data.Value = GetLocation();
                FiveDays weather = new FiveDays();
                weather.Show();
            }
        }
    }
}
