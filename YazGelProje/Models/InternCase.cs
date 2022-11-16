using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class InternCase
    {
        [Key]
        public int InternCaseId { get; set; }
        public string Case { get; set; }
        public bool? Hidden { get; set; }
    }
}