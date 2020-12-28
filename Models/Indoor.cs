using System;
using System.Collections.Generic;

#nullable disable

namespace TemperatureWPF.Models
{
    public partial class Indoor
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Temperature { get; set; }
        public int? Humidity { get; set; }
    }
}
