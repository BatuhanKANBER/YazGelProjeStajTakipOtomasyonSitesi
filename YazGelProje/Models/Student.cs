using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string Tc { get; set; }
        [Required]
        public string StudentNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string PhoneNumber { get; set; }
        public bool? ToApply { get; set; }
        public string Adress { get; set; }
        public int? InternCaseId { get; set; }
        public virtual InternCase InternCase { get; set; }
    }
}