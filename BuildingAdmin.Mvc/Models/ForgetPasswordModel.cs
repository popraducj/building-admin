using System.ComponentModel.DataAnnotations;

namespace BuildingAdmin.Mvc.Models
{
    public class ForgotPasswordModel{

        [Required(ErrorMessage="The email field is mandatory")]
        [EmailAddress(ErrorMessage="This email address is not valid")]
        [Display(Prompt="Email")]
        public string Email{get;set;}
    }
}