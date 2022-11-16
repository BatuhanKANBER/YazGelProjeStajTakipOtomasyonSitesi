using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class InternBookToGive
    {
        [Key]
        public int InternBookToGiveId { get; set; }
        public string PageName { get; set; }
        public DateTime? Date { get; set; }
        public int? StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}