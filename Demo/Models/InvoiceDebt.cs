using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class InvoiceDebt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceNo { get; set; }

        public int AccCode {get;set;}
        public virtual DebtorMaster DebtorMaster { get; set; }

        public int Tid { get; set; }

        public string Stock { get; set; }
        
        public double UnitPrice { get; set; }

        public int quantity1 { get; set; }

        public double TotalExl { get; set; }

        public double VatAmo { get; set; }

        public double Total { get; set; }

        public DateTime InvoiceDate { get; set; }
    }
}