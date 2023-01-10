using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class CreditCard
    {

        [Key]
        [Required(ErrorMessage = "Credit card number is required")]
        public string credit_num { get; set; }

        [Required(ErrorMessage = "CVC number is required")]
        public string cvc { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string full_name { get; set; }

        [Required(ErrorMessage = "Payer ID is required")]
        public string payer_id { get; set; }

        public float balance { get; set; }

    }
}