using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class SettingsChangePassword : ChangePassword
    {        
        [Required(ErrorMessage="The old password field is mandatory")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password should be between {2} and {1} characters long", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$",
             ErrorMessage = "Password Pattern Error")]             
        [Display(Prompt="Old Password")]
        public string OldPassword {get;set;}
    }
}