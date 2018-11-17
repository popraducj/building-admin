using System;
using System.ComponentModel.DataAnnotations;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Mvc.Models
{
    public class ServicesModel
    {
        [Required(ErrorMessage="The provider name field is mandatory")]
        [Display(Name="Provider name")]
        [StringLength(500, ErrorMessage = "The name should be between {2} and {1} characters long", MinimumLength = 3)]
        public string Name { get; set;}

        [Required(ErrorMessage="The unit field is mandatory")]
        [Display(Name="Unit")]
        [StringLength(100, ErrorMessage = "The name should be between {2} and {1} characters long", MinimumLength = 1)]
        public string Unit { get; set; }
        [Display(Name="Display name for apartments")]
        public string DisplayNameForApartments { get; set; }

        [Display(Name="With personal reading")]
        public bool WithPersonalReading { get; set; }
        [Display(Name="Default value")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float DefaultValue { get; set; } = 0;
        public Guid BuildingId { get; set; }
        public Guid Id {get; set;}
        public DateTime AddedAtUtc {get;set;}
        
        public ServicesModel() {}
        public ServicesModel(Service service, Guid buildingId){
            Name = service.Name;
            Unit = service.Unit;
            BuildingId = buildingId;
            DisplayNameForApartments = service.DisplayNameForApartments;
            WithPersonalReading = service.WithPersonalReading;
            DefaultValue = service.DefaultValue;
            AddedAtUtc = service.AddedAtUtc;
        }
        public Service MapToDatabase(){
            return new Service{
                Id = Id,
                Name = Name,
                Unit = Unit,
                DisplayNameForApartments = WithPersonalReading ? DisplayNameForApartments : string.Empty,
                WithPersonalReading = WithPersonalReading,
                DefaultValue = WithPersonalReading ? 0 : DefaultValue,
                AddedAtUtc = AddedAtUtc
            };
        }
    }
}