using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class InternStudentStart
    {
        [Key]
        public int InternStudentStartId { get; set; }
        public string WorkPlaceName { get; set; }
        public DateTime? InternStart { get; set; }
        public DateTime? InternEnd { get; set; }
        public int? WeekDayCount { get; set; }
        public int? HolidayCount { get; set; }
        public int? WorkTime { get; set; }
        public DateTime? Date { get; set; }
        public string SaturdayWork { get; set; }
        public int? StudentId { get; set; }
        public string WorkPlaceAdress { get; set; }
        public virtual Student Student { get; set; }
    }
}