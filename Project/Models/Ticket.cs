using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Ticket
    {
        
        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "Flight number is required")]
        public int flight_num { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        public int num_of_tickets { get; set; }

        [Key]
        [Required(ErrorMessage = "Ticket ID is required")]
        public int ticket_id { get; set; }

    }
}