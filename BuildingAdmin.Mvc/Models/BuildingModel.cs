using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class BuildingModel
    {
        [Required(ErrorMessage="The name field is mandatory")]
        [Display(Name="Name")]
        [StringLength(500, ErrorMessage = "The name should be between {2} and {1} characters long", MinimumLength = 3)]
        public string Name { get; set;}
        [Required(ErrorMessage="The address field is mandatory")]
        [Display(Name="Address")]
        [StringLength(500, ErrorMessage = "The address should be between {2} and {1} characters long", MinimumLength = 3)]
        public string Address { get; set; }
        [Required(ErrorMessage="The CUI field is mandatory")]
        [Display(Name="CUI")]
        [StringLength(10, ErrorMessage = "The CUI should be between {2} and {1} characters long", MinimumLength = 3)]
        public string CUI { get; set; }
        [Display(Name="Penalities")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float Penalities { get; set; }
        [Display(Name="IBAN")]
        public string IBAN { get; set; }
        public Guid Id {get;set;}
        public List<ApartmentModel> Apartments { get; set; }
        public ApartmentModel NewApartmentModel { get; set; }
        public BuildingModel(){}

        public BuildingModel(Building building){
            Map(building);
        }

        public void Map(Building building){
            Name = building.Name;
            Address = building.Address;
            CUI = building.CUI;
            Penalities = building.Penalities;
            IBAN = building.IBAN;
            Id = building.Id;
        }

        public Building MapToDatabase(){
            return new Building(){
                Name = Name,
                Address = Address,
                CUI = CUI,
                Penalities = Penalities,
                IBAN = IBAN,
                Id = Id
            }; 
        }
    }
}