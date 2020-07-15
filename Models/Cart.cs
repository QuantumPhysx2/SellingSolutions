using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SellingSolutions.Models
{
    public class Cart
    {
        public int ID { get; set; }

        [Display(Name = "Cart Number")]
        public string CartID { get; set; }

        [Display(Name = "Item")]
        public int ItemID { get; set; }

        [Range(0, 10000)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Inc GST")]
        public decimal PriceGST { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public decimal PriceTotal { get; set; }
    }
}
