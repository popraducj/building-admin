using System;
using System.Linq;
using System.Security.Claims;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BuildingAdmin.Mvc.Controllers{
    public class BaseController : Controller{
        private readonly IBuildingService _buildingService;

        public BaseController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }
        protected Building GetBuilding(Guid buildingId)
        {
            var buildings = _buildingService.GetAllByOwner(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(TempData["SelectedBuilding"] != null){
                buildingId = (Guid)TempData["SelectedBuilding"];
            }
            var newGuid = new Guid();
            buildingId = buildingId == newGuid ? buildings.Select(b => b.Id).FirstOrDefault() : buildingId;
            var building = buildings.Where(b => b.Id.Equals(buildingId)).FirstOrDefault() ?? new Building();            
            
            ViewBag.Buildings = buildings;
            ViewBag.SelectedBuilding = buildingId.ToString();

            return building;
        }

        protected bool IsValidBuilding(Guid buildingId){
            if (InvalidGuid(buildingId)){
                return false;
            }

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), buildingId);
            return building != null;
        }
        
        protected bool InvalidGuid(Guid id) => id == null || id.Equals(new Guid().ToString());
    }
}