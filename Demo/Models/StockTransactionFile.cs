using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class StockTransactionFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockTransactionID { get; set; }

        [Required(ErrorMessage = "Please Select Item")]
        public int StockCode { get; set; }
        public string StockDes { get; set; }
        public virtual StockMaster StockMaster { get; set; }

        [Required(ErrorMessage = "Please enter qauntity")]
        public int Quantity { get; set; }

       
        [Required(ErrorMessage = "Please enter qauntity")]
        public double UnitCost { get; set; }

        public double UnitSP { get; set; }
         public double Total { get; set; }
        public DateTime StockTransactionDate { get; set; }

      
       


    }
}