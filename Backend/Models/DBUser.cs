using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class DBUser : IdentityUser
    {
        public bool IIA { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Count Money be between 0 and long.MaxValue")]
        public long Money { get; set; }
        public ICollection<DBUserBond> Bonds { get; set; }
        [Range(0, 10, ErrorMessage = "Risk must be between 0 and 10")]
        [Index]
        public int Risk { get; set; }
        [Column("monthly_investment")]
        public long MonthlyInvestment { get; set; }
        public bool Reinvestment { get; set; }
    }
}
