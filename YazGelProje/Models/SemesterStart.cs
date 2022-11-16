using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class SemesterStart
    {
        [Key]
        public int Id { get; set; }
        public string Case { get; set; }
        public string Description { get; set; }
    }
}