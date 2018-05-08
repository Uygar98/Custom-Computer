using Custom_Computers.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Custom_Computers.Models.ViewModels.Account
{
    public class UserView
    {

        public UserView()
        {

        }

        public UserView(UserDTO row)
        {
            Id = row.Id;
            Firstname = row.Firstname;
            Lastname = row.Lastname;
            EmailAddress = row.EmailAddress;
            Username = row.Username;
            Password = row.Password;
            

        }

        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]

        public string ConfrimPassword { get; set; }
    }
}