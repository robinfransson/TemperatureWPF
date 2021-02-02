using System;

namespace TemperatureWPF
{
    /// <summary>
    /// Beroende på context är value temperatur eller luftfuktighet
    /// </summary>
    class DataHolder
    {
        public string Date { get; set; }
        public double Value { get; set; }


        public DataHolder(DateTime date, double value)
        {
            Date = date.ToString("d");
            Value = value;
        }
    }
}
