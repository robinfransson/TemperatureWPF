using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureWPF
{
    class MoldChance
    {
        public double? Humidity { get; set; }
        public double? Temperature { get; set; }
        public string Date { get; set; }
        public MoldChance(string date, double? humidity, double? temperature)
        {
            Date = date;
            Humidity = humidity;
            Temperature = temperature;
        }
    }
}
