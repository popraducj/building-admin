using System.Security.Claims;
using System.Linq;
using BuildingAdmin.DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;
using System.Threading.Tasks;
using System.Collections.Generic;
using BuildingAdmin.Mvc.Models;
using System;
using BuildingAdmin.Services.Contracts;

namespace BuildingAdmin.Mvc.Controllers
{
    [Authorize(Policy ="admin")]
    public class ApartmentController : BaseController
    {
        private readonly IBuildingService _buildingService;
        private readonly IApartmentService _apartmentService;

        public ApartmentController(IBuildingService buildingService, IApartmentService apartmentService) : base(buildingService)
        {
            _buildingService = buildingService;
            _apartmentService = apartmentService;
        }

        public ActionResult List(Guid id){
            var apartments = new List<Apartment>();
            var building = GetBuilding(id);

            if(building.Apartments.Count == 0){
                return View(apartments);
            }

            apartments = _apartmentService.GetAllFromBuilding(building);
            return  View(apartments);
        }

        public ActionResult Add(Guid buildingId){
            if (!IsValidBuilding(buildingId)){
                return StatusCode(404);
            }

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), buildingId);
            return View(new ApartmentModel{
                BuildingId = buildingId,
                Number = building.Apartments.Count + 1
            });
        }

        [HttpPost]
        public async Task<ActionResult> Add(ApartmentModel apartment) {

            if (!ModelState.IsValid){
                return View(apartment);
            }
            var databaseApartment = apartment.MapToDatabase();
            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), apartment.BuildingId);

            databaseApartment.Id = Guid.NewGuid();
            databaseApartment.AddHistoryNewItems(apartment.FirstName, apartment.LastName, apartment.NoOfPersons);
            databaseApartment.Number = building.Apartments.Count + 1;
            await _apartmentService.SaveAsync(databaseApartment);

            building.Apartments.Add(databaseApartment.Id);
            await _buildingService.UpdateAsync(building);

            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

        public ActionResult Edit(Guid id){
            if(InvalidGuid(id)){
                return StatusCode(404);
            }

            var building = _buildingService.GetByOwnerAndApartmentId(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            if(building == null){
                return StatusCode(404);
            }

            var apartment = _apartmentService.GetById(id);
            
            if(apartment == null){
                return StatusCode(404);
            }
            return View(new ApartmentModel(apartment, building.Id));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ApartmentModel apartment) {

            if (!ModelState.IsValid){
                return View(apartment);
            }

            var databaseApartment = _apartmentService.GetById(apartment.Id);
            if(databaseApartment == null){
                return StatusCode(404);
            }
            
            databaseApartment.AddToHistory(apartment.FirstName, apartment.LastName, apartment.NoOfPersons);
            var apartmentForDatabase = apartment.MapToDatabase();
            apartmentForDatabase.FirstName = databaseApartment.FirstName;
            apartmentForDatabase.LastName = databaseApartment.LastName;
            apartmentForDatabase.NoOfPersons = databaseApartment.NoOfPersons;
            await _apartmentService.UpdateAsync(apartmentForDatabase);
            
            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), apartment.BuildingId);
            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

    }
}