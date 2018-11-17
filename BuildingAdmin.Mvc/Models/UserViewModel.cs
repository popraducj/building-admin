using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingAdmin.Mvc.Models
{
    public class UserViewModel  
    {       
        [Required(ErrorMessage="The email field is mandatory")]
        [EmailAddress(ErrorMessage="This email address is not valid")]
        [Display(Prompt="Email")]
        public string Email {get;set;}
        
        [Required(ErrorMessage="The password field is mandatory")]
        [DataType(DataType.Password)]
        [Display(Prompt="Password")]
        public string Password {get;set;}

        public bool RememberMe{get;set;}
    }
}