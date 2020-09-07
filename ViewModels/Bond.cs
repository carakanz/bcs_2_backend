using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Bond
    {
        [Key]
        public int Id { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Count must be between 0 and long.MaxValue")]
        public long? Count { get; set; }

        [Required]
        public Company Company { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Priсe must be between 0 and long.MaxValue")]
        [Column("purchase_price")]
        public long PurchasePrice { get; set; }

        [Required]
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Priсe must be between 0 and long.MaxValue")]
        [Column("selling_price")]
        public long SellingPrice { get; set; }

        [Required]
        [Column("selling_date")]
        public DateTime SellingDate { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Priсe must be between 0 and long.MaxValue")]
        [Column("current_purchase_priсe")]
        public long? CurrentPurchasePrice { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Priсe must be between 0 and long.MaxValue")]
        [Column("current_selling_price")]
        public long? CurrentSellingPrice { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Risk must be between 0 and 10")]
        public int Risk { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "InterestRate must be between 0 and int.MaxValue")]
        [Column("interest_rate")]
        public int InterestRate { get; set; }

        [Column("coupon_frequency")]
        [Required]
        [Range(0, 365, ErrorMessage = "Coupon frequency must be between 0 and 365")]
        public int CouponFrequency { get; set; }
    }
}
