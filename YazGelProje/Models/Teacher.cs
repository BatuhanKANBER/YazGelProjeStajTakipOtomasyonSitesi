using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public string SicilNo { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public int? CommissionId { get; set; }
        public virtual Commission Commission { get; set; }
    }
}