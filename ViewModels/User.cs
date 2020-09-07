using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace ViewModels
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        public bool IIA { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Count Money be between 0 and long.MaxValue")]
        public long Money { get; set; }

        public IEnumerable<Bond> Bonds { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Risk must be between 0 and 10")]
        public int Risk { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Count must be between 0 and long.MaxValue")]
        public long MonthlyInvestment { get; set; }

        [Required]
        public bool Reinvestment { get; set; }
    }
}
