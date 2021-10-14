using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Demo.Models
{
    public class DebtorMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccCode { get; set; }

        [Required]
        [Display(Name = "Debtor Name")]
        public string DebtorName { get; set; }

        [Required]
        [Display(Name = "Debtor surname")]
        public string DebtorSurname { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        [StringLength(13, ErrorMessage = "The ID number is invalid, it should be 13 digits long", MinimumLength = 13)]
        public string IDNo { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string DebtorsAddress { get; set; }

        [Required]
        [Display(Name = "Suburb")]
        public string Suburb { get; set; }

        [Required]
        [Display(Name = "Town")]
        public string Town { get; set; }

        [Required]
        [Display(Name = "Province")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [StringLength(10, ErrorMessage = "The contact number is invalid, it should be 10 digits long", MinimumLength = 10)]
        public string CellNo { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string email { get; set; }

        public double Balance { get; set; }
        public double SalesToDate { get; set; }

        public double CostToDate { get; set; }



    }
}