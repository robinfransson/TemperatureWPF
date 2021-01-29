using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureWPF
{
    class MoldRisk
    {

        public string Date { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public double RiskPercent { get; set; }




        public MoldRisk(DateTime date, double humidity, double temperature)
        {
            Date = date.ToString("d");
            Humidity = Math.Round(humidity, 1);
            Temperature = Math.Round(temperature, 1);
            RiskPercent = CalculateMoldRisk(humidity, temperature);
        }





        private double CalculateMoldRisk(double hum, double temp)
        {
            double result = (hum - 78) * (temp / 15) / 0.22; // resultatet (mögelrisken) i procent
            if (result > 100)
            {
                result = 100;
            }
            else if (result < 0)
            {
                result = 0;
            }
            return Math.Round(result, 0);
        }
    }
}
