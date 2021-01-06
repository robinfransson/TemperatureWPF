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

        public DateTime dt { get; set; }


        public DataHolder(double? value, string date)
        {
            Value = value;
            Date = date;
        }

        public DataHolder(double? value, string date, DateTime dtc)
        {
            Value = value;
            Date = date;
            dt = dtc;
        }
    }
}
