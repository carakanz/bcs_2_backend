using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ViewModels
{
    public class UserBondBuyRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int BondId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
