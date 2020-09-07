using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    [Table("user_bond")]
    public class DBUserBond
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public string UserId { get; set; }

        [Required]
        public int BondId { get; set; }
        [Required]
        public DBBond Bond { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Count must be between 0 and long.MaxValue")]
        public long Count { get; set; }
    }
}
