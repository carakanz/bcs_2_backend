using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AutorisationLoginResponse
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
