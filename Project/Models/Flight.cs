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
        public float price { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 50 characters")]
        public string destination_country { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 50 characters")]
        public string origin_country { get; set; }

        public DateTime date_time { get; set; }
        public int num_of_seats { get; set; }

    }
}