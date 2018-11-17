using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingAdmin.Mvc.Models
{
    public class UserRegistration  
    {
        [Required(ErrorMessage="The password field is mandatory")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password should be between {2} and {1} characters long", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$",
             ErrorMessage = "Password Pattern Error")]             
        [Display(Prompt="Password")]
        public string Password {get;set;}

        [Required(ErrorMessage="The confirm password field is mandatory")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage="The password and confirm password should be the same")]
        [Display(Prompt="Confirm Password")]   
        public string ConfirmPassword {get;set;}

        [Required(ErrorMessage="The email field is mandatory")]
        [RegularExpression(@"^((?("")("".+?(?<!\\)""@(?<=^.{1,64}@))|(([0-9a-zA-Z]((\.(?!\.))|[-!#$%&'*+/=?^`{}|~\w])*)@(?<=[0-9a-zA-Z]@)(?<=^.{1,64}@))))(?(\[)(\[(?:\d{1,3}\.){3}\d{1,3}])|(([0-9a-zA-Z][-0-9a-zA-Z]*[0-9a-zA-Z]*\.)+[a-z0-9A-Z][-a-z0-9A-Z]{0,22}[a-z0-9A-Z]))$"
            , ErrorMessage = "This email address is not valid")]
        [Display(Prompt="Email")]
        public string Email { get; set; }

        [Required(ErrorMessage="The first name field is mandatory")]
        [StringLength(100, ErrorMessage = "The first name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="The last name field is mandatory")]
        [StringLength(100, ErrorMessage = "The last name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="Last Name")]
        public string LastName { get; set; }

        public bool Captcha {get;set;}
        public bool Terms {get;set;} = true;

    }
}