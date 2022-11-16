using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }
        public DateTime? HolidayDate { get; set; }
        public string Description { get; set; }
    }
}