using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class SettingsModel
    {
        [Required(ErrorMessage="The first name field is mandatory")]
        [StringLength(100, ErrorMessage = "The first name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="The last name field is mandatory")]
        [StringLength(100, ErrorMessage = "The last name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="Last Name")]
        public string LastName { get; set; }


        public SettingsModel() {}
        public SettingsModel(UserModel userSettings){
            Map(userSettings);
        }
        public void Map(UserModel userSettings){            
            FirstName = userSettings.FirstName;
            LastName = userSettings.LastName;
        }
    }
}