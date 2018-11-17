using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class ProfileModel
    {
        [Required(ErrorMessage="The first name field is mandatory")]
        [StringLength(100, ErrorMessage = "The first name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="The last name field is mandatory")]
        [StringLength(100, ErrorMessage = "The last name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Prompt="Last Name")]
        public string LastName { get; set; }
        [Display(Prompt="Description")]
        public string Description { get; set; }

        [Display(Prompt="Address")]
        public string Address { get; set; }

        [Display(Prompt="Phone number")]
        public string Phone { get; set; }
        
        public string Role {get;set;}

        public ProfileModel() {}
        public ProfileModel(UserModel userProfile){
            Map(userProfile);
        }
        public void Map(UserModel userProfile){
            Phone = userProfile.Phone;
            Address = userProfile.Address;
            Description = userProfile.Description;
            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            Role = string.Join(",",userProfile.Roles);
        }
    }
}