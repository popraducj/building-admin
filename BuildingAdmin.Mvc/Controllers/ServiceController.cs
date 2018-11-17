using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    [Authorize(Policy ="admin")]
    public class ServiceController : BaseController
    {
        private IStringLocalizer<ServiceController> _localzier;
        private readonly IBuildingService _buildingService;
        private readonly IServicesService _servicesService;

        public ServiceController(IStringLocalizer<ServiceController> localzier, IBuildingService buildingService, IServicesService servicesService)
                : base(buildingService)
        {
            _localzier = localzier;
            _buildingService = buildingService;
            _servicesService = servicesService;
        }

        public IActionResult List(Guid id){
            var services = new List<Service>();
            var building = GetBuilding(id);

            if(building.Services.Count == 0){
                return View(services);
            }

            services = _servicesService.GetAllByBuilding(building);
            return  View(services);
        }

        public ActionResult Add(Guid id){
            if(!IsValidBuilding(id)){
                return StatusCode(404);
            }

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            ViewBag.OtherServices = _servicesService.GetAllNotSetToBuilding(building.Services, building.Owner);

            return View(new ServicesModel{
                BuildingId = id
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddService(Guid id, Guid buildingId) {

            if(InvalidGuid(id) || !IsValidBuilding(buildingId) || _servicesService.GetById(id) == null) return StatusCode(404);

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), buildingId);
            building.Services.Add(id);
            await _buildingService.UpdateAsync(building);
            
            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }
        
        [HttpPost]
        public async Task<ActionResult> Add(ServicesModel service) {

            ValidateModel(service);
            if (!ModelState.IsValid){
                return View(service);
            }
            
            var databaseService = service.MapToDatabase();
            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), service.BuildingId);

            databaseService.Id = Guid.NewGuid();
            databaseService.AddedAtUtc = DateTime.Now;
            databaseService.Owner = building.Owner;
            await _servicesService.SaveAsync(databaseService);

            building.Services.Add(databaseService.Id);
            await _buildingService.UpdateAsync(building);
            
            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

        public IActionResult Edit(Guid id, Guid buildingId){
            if(InvalidGuid(id) || !IsValidBuilding(buildingId)) return StatusCode(404);

            var service = _servicesService.GetById(id);            
            if(service == null) return StatusCode(404);

            return View(new ServicesModel(service, buildingId));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ServicesModel service) {

            if (!ModelState.IsValid){
                return View(service);
            }

            var databaseService = _servicesService.GetById(service.Id);
            if(databaseService == null){
                return StatusCode(404);
            }
            service.AddedAtUtc= databaseService.AddedAtUtc;
            var serviceForDatabase = service.MapToDatabase();
            await _servicesService.UpdateAsync(serviceForDatabase);
            
            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), service.BuildingId);
            
            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

        public async Task<ActionResult> Remove(Guid id, Guid buildingId)
        {
            if(InvalidGuid(id) || !IsValidBuilding(buildingId)) return StatusCode(404);

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), buildingId);
            building.Services.Remove(id);
            await _buildingService.UpdateAsync(building);

            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

        private void ValidateModel(ServicesModel service)
        {
            if (service.WithPersonalReading && string.IsNullOrEmpty(service.DisplayNameForApartments))
            {
                ModelState.AddModelError("DisplayNameForApartments", _localzier["The display name for apartments is mandatory"]);
                return;
            }
            if (service.DefaultValue < 0)
            {
                service.DisplayNameForApartments = string.Empty;
                ModelState.AddModelError("DefaultValue", _localzier["Default vallue cannot be less than 0"]);
            }
        }
    }
}