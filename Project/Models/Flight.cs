using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Flight
    {
        [Key]
        [Required(ErrorMessage = "flight number is required")]
        public int flight_num { get; set; }
        
        [Required(ErrorMessage = "price is required")]
        public float price { get; set; }
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 50 characters")]
        [Required(ErrorMessage = "destination country is required")]
        public string destination_country { get; set; }
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 50 characters")]
        [Required(ErrorMessage = "origin country is required")]
        public string origin_country { get; set; }
        
        public DateTime date_time { get; set; }

        [Required(ErrorMessage = "number of seats is required")]
        public int num_of_seats { get; set; }

    }
}