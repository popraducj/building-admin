using System.ComponentModel.DataAnnotations;

namespace BuildingAdmin.Mvc.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage="The password field is mandatory")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password should be between {2} and {1} characters long", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$",
             ErrorMessage = "Password Pattern Error")]
             //"Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]      
        [Display(Prompt="Password")]
        public string Password {get;set;}

        [Required(ErrorMessage="Password confirmation is mandatory")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage="The password and confirm password should be the same")]      
        [Display(Prompt="Confirm Password")]
        public string ConfirmPassword {get;set;}
        public string Token {get;set;}
    }
}