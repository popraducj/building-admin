using System.ComponentModel.DataAnnotations;

namespace BuildingAdmin.Mvc.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage="The name field is mandatory")]
        [Display(Prompt="Name")]
        [StringLength(100, ErrorMessage = "The name should be between {2} and {1} characters long", MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage="The email field is mandatory")]
        [EmailAddress(ErrorMessage="This email address is not valid")]
        [Display(Prompt="Email")]
        public string Email { get; set; }

        [Required(ErrorMessage="The message field is mandatory")]
        [StringLength(500, ErrorMessage = "The message should be between {2} and {1} characters long", MinimumLength = 20)]
        [Display(Prompt="Message")]
        public string Message { get; set; }
    }
}