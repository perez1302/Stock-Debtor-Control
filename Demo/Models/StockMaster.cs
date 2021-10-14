using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class StockMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockCode { get; set; }

        [Required]
        [Display(Name = "Stock Description")]
        public string StockDes { get; set; }

        [Required]
        [Display(Name = "Item Cost R")]
        public double Cost { get; set; }

        [Required]
        [Display(Name = "Mark up %")]
        public double MarkUp { get; set; }

        public double SellingPrice { get; set; }


        [Required]
        [Display(Name = "Quantity Purchasing")]
        public double QtyPurchased { get; set; }

     
        public double TotalPurchase { get; set; }

       
        public double QtySold { get; set; }

        public double TotalSales { get; set; }


        public double StockAvailable { get; set; } //Stock on Hand

    }
}