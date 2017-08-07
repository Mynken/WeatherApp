using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.FiveDaysWeather
{
    public class FiveDays
    {
        public class Main
        {
            public double _temp;
            public double temp
            {
                get
                {
                    return _temp;
                }
                set
                {
                    _temp = value - 273.15; // In Celsij
                }
            }

        }
        public class Weather
        {
            public string main { get; set; }
        }
        public class All
        {
            public Main main { get; set; }
            public string dt_txt { get; set; }
            public List<Weather> weather { get; set; }
        }

        public class City
        {
            public string name { get; set; }
            public string country { get; set; }
        }

        public class RootObject
        {
            public List<All> list { get; set; }
            public City city { get; set; }
        }
    }
}
