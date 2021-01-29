using System;

namespace TemperatureWPF
{
    /// <summary>
    /// Beroende på context är value temperatur eller luftfuktighet
    /// </summary>
    class DataHolder
    {
        public double Value { get; set; }
        public string Date { get; set; }


        public DataHolder(double value, DateTime date)
        {
            Value = value;
            Date = date.ToString("d"); ;
        }
    }
}
