using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Company
    {
        [Key]
        [Required]
        [StringValidator(MinLength = 2)]
        public string Name { get; set; }

        [Required]
        [Url]
        [DataType(DataType.Url)]
        public string URL { get; set; }

        [Required]
        [Url]
        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [DataType(DataType.MultilineText)]
        public string Destription { get; set; }
    }
}
