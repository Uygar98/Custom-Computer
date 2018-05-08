using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.ViewModels.Account
{
    public class LoginUserView
    {
        [Required]
        public string Username { get; set; }
        [Required]

        public string Password { get; set; }
        public bool RememberMe { get; set; }



    }
}