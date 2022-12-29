using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters")]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^[A-Za-z0-9]{4,16}$", ErrorMessage = "Password can contain digits or letters and must be between 4 and 16 characters")]
        public string password { get; set; }

        public int admin { get; set; }

    }
}