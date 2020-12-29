using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureWPF
{
    class DataHolder
    {
        public double? Value { get; set; }
        public string Date { get; set; }


        public DataHolder(double? value, string date)
        {
            Value = value;
            Date = date;
        }
    }
}
