using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TemperatureWPF.Models
{
    [Table("Indoor")]
    public partial class Indoor
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Temperature { get; set; }
        public int? Humidity { get; set; }
    }
}
