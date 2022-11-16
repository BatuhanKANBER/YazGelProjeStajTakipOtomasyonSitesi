using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YazGelProje.Models
{
    public class Commission
    {
        [Key]
        public int CommissionId { get; set; }
        public string Title { get; set; }
    }
}