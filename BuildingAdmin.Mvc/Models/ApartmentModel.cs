using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class ApartmentModel
    {
        [Display(Name="Number")]
        public int Number { get; set; }
        [Display(Name="Phone")]
        public string OwnerPhone { get; set; }
        [EmailAddress(ErrorMessage="This email address is not valid")]
        [Display(Name="Email")]
         public string OwnerEmail { get; set; }
        [Required(ErrorMessage="The cut field is mandatory")]
        [Display(Name="Cut")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float Cut { get; set; }
        [Required(ErrorMessage="The number of persons field is mandatory")]
        [Display(Name="Number of persons")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public byte NoOfPersons { get; set; }
        
        [Required(ErrorMessage="The first name field is mandatory")]
        [StringLength(100, ErrorMessage = "The first name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="The last name field is mandatory")]
        [StringLength(100, ErrorMessage = "The last name should be between {2} and {1} characters long", MinimumLength = 3)]
        [Display(Name="Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage="The entrance field is mandatory")]
        [Display(Name="Entrance")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public byte Entrance { get; set; }
        public Guid Id { get; set; }
        
        public Guid BuildingId { get; set; }

        public ApartmentModel() {}

        public ApartmentModel(Apartment apartment, Guid buildingId){
            Number = apartment.Number;
            OwnerEmail = apartment.OwnerEmail;
            OwnerPhone = apartment.OwnerPhone;
            Cut  = apartment.Cut;
            Id = apartment.Id;
            BuildingId = buildingId;
            Entrance = apartment.Entrance;
            if(apartment.FirstName != null  && apartment.FirstName.Count > 0){
                FirstName = apartment.FirstName.Last().Value;
            }
            if(apartment.LastName != null  && apartment.LastName.Count > 0){
                LastName = apartment.LastName.Last().Value;
            }
            if(apartment.NoOfPersons != null  && apartment.NoOfPersons.Count > 0){
                NoOfPersons = byte.Parse(apartment.NoOfPersons.Last().Value);
            }
        }

        public Apartment MapToDatabase(){
            return new Apartment{
                Number = Number,
                OwnerEmail =OwnerEmail,
                OwnerPhone = OwnerPhone,
                Cut = Cut,
                Id = Id,
                Entrance = Entrance
            };
        }
    }
}