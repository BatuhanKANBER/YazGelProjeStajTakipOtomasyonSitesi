using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class Intern
    {
        [Key]
        public int InternId { get; set; }
        public string Title { get; set; }
        public string Desciription { get; set; }
        public DateTime? InternDate { get; set; }
        public int? StudentId { get; set; }
        public int? InternSemesterNumber { get; set; }
        public virtual Student Student { get; set; }
    }
}