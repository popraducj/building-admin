using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    [Authorize(Policy ="admin")]
    public class BuildingController : BaseController
    {
        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService): base(buildingService)
        {
            _buildingService = buildingService;
        }

        public ActionResult List(){
            
            var buildings = _buildingService.GetAllByOwner(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(buildings);
        }

        public ActionResult Add()
        {
            return View(new BuildingModel());
        }

        [HttpPost]
        public async Task<ActionResult> Add(BuildingModel model)
        {
            if(!ModelState.IsValid){
                return View(model);
            }
            await SaveBuilding(model);
            //return a success view
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<ActionResult> AddAndGoToApartments(BuildingModel model)
        {
            if(!ModelState.IsValid){
                return View(model);
            }
            var id = await SaveBuilding(model);
            var buildings = _buildingService.GetAllByOwner(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            ViewBag.Buildings = buildings;
            ViewBag.SelectedBuilding = id.ToString();
            return View("~/Views/Apartment/List.cshtml", new List<Apartment>());
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            return View(new BuildingModel(building));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(BuildingModel model)
        {
            if(!ModelState.IsValid){
                return View(model);
            }
            var building =  _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), model.Id);
            building.Penalities = model.Penalities;
            building.IBAN = model.IBAN;
            await _buildingService.UpdateAsync(building);
            //return a success view
            return RedirectToAction(nameof(List));
        }
        
        public async Task<ActionResult> Remove(Guid id)
        {
            if(!IsValidBuilding(id)) return StatusCode(404);

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            building.IsActive = false;
            await _buildingService.UpdateAsync(building);

            return RedirectToAction(nameof(List));
        }

        private async Task<Guid> SaveBuilding(BuildingModel model){
            var building = model.MapToDatabase();
            building.Owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            building.AddedAtUtc = DateTime.Now;
            building.Id = Guid.NewGuid();
            building.IsActive = true;
            await _buildingService.SaveAsync(building);
            return building.Id;
        }
    }
}