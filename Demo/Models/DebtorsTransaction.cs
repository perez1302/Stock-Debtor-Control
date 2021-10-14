using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class DebtorsTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        public int AccCode { get; set; }
        public virtual DebtorMaster DebtorMaster { get; set; }

        [Required(ErrorMessage = "Please Select Item")]
        public int StockCode { get; set; }
        public string StockDes { get; set; }
        public double SellingPrice { get; set; }
        public virtual StockMaster StockMaster{get;set;}

        [Required(ErrorMessage ="Please enter Qty")]
        public int Qty { get; set; }
        public double GrossValue { get; set; }
        public double VatValue { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}