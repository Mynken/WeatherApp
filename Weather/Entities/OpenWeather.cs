using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Entities;

namespace Weather
{
    public class OpenWeather
    {
        public Coord coord;
        public weather[] weather;
        
        [JsonProperty("base")]
        public string Base;
        public Main main;
        
        public double visibility;
        public Wind wind;
        public Clouds clouds;
        public double dt;
        public Sys sys;
        public int id;
        public string name;
        public double cod;
    }
}
