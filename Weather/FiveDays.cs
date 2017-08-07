using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weather.FiveDaysWeather;
using static Weather.FiveDaysWeather.FiveDays;

namespace Weather
{
    public partial class FiveDays : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public FiveDays()
        {
            InitializeComponent();
            pictureBox1.Image = Properties.Resources._default;
            pictureBox1.SendToBack();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Dock = DockStyle.Fill;

            listView1.Parent = pictureBox1;
            //  listView1.BackColor = Color.Transparent;

            button1.Parent = pictureBox1;
            button1.BackColor = Color.Transparent;

            List<Label> list = new List<Label>() { label1 };
            foreach (var item in list)
            {
                item.Parent = pictureBox1;
                item.BackColor = Color.Transparent;
            }
        }
        
        public async Task<string> GetWeatherForCityAsync(string city)
        {
            WebRequest request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/forecast?q={city}&APPID=133f53a4ef2155e4c0e5054102d7b8b1");
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

        private async void FiveDays_LoadAsync(object sender, EventArgs e)
        {
            try
            {
                RootObject result = JsonConvert.DeserializeObject<RootObject>(await GetWeatherForCityAsync(Data.Value));
                label1.Text = "Weather for " + result.city.name.ToString() + " " + result.city.country.ToString();

                for (int i = 0; i <= 39; i++)
                {
                    ListViewItem lv = new ListViewItem(result.list[i].dt_txt.ToString());
                    if (!result.list[i].main.temp.ToString().Contains("-"))
                    {
                        lv.SubItems.Add(result.list[i].main.temp.ToString("+ 0.## °C"));
                    }
                    else
                    {
                        lv.SubItems.Add(result.list[i].main.temp.ToString(" 0.## °C"));
                    }

                    lv.SubItems.Add(result.list[i].weather[0].main.ToString());

                    listView1.Items.Add(lv);
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
