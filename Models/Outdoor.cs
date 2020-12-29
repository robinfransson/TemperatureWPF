using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TemperatureWPF.Models
{
    [Table("Outdoor")]
    public partial class Outdoor
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Temperature { get; set; }
        public int? Humidity { get; set; }
    }
}
