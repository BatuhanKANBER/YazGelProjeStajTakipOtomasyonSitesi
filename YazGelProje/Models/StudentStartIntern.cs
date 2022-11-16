using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class StudentStartIntern
    {
        [Key]
        public int StudentStartInternId { get; set; }
        public string WorkPlace { get; set; }
        public DateTime? InternStart { get; set; }
        public DateTime? InternEnd { get; set; }
        public int? Week { get; set; }
        public int? Holiday { get; set; }
        public int? WorkTime { get; set; }
        public DateTime? Date { get; set; }
        public string WeekendWorking { get; set; }
        public int? InternNameId { get; set; }
        public int? StudentId { get; set; }
        public string WorkPlaceAdress { get; set; }
        public virtual Student Student { get; set; }
        public virtual InternName InternName { get; set; }
    }
}