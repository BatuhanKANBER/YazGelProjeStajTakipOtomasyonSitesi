using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class InternFile
    {
        [Key]
        public int FileId { get; set; }
        public string FileName { get; set; }
        public DateTime? FileDate { get; set; }
        public int? StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}