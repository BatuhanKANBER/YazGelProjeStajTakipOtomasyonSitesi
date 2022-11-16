using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class ToBackFile
    {
        [Key]
        public int ToBackFileId { get; set; }
        public string FileName { get; set; }
        public DateTime? FileDate { get; set; }
        public int? StudentId { get; set; }
        public string Description { get; set; }
        public virtual Student Student { get; set; }
    }
}