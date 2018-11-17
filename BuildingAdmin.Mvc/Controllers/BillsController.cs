using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers{
    [Authorize(Policy ="admin")]
    public class BillController : BaseController{
        private readonly IBillService _billService;
        private readonly IBuildingService _buildingService;
        private readonly IServicesService _servicesService;

        public BillController(IBillService billService, IBuildingService buildingService, IServicesService servicesService) : base(buildingService){
            _billService = billService;
            _buildingService = buildingService;
            _servicesService = servicesService;
        }

        public IActionResult List(Guid id){
            var bills = new List<Bill>();
            var building = GetBuilding(id);

            ViewBag.EnableAddNewBill = true;
            ViewBag.InvalidAdd = false;
            if(building.Bills.Count == 0){
                return View(bills);
            }

            bills = _billService.GetAllFromBuilding(building);
            ViewBag.EnableAddNewBill = bills.Where(x => x.End == null).FirstOrDefault() == null;
            return  View(bills);
        }

        public IActionResult Get(int month = 0)
        {
           return View(nameof(Get));
        }

        public IActionResult Add(Guid buildingId)
        {
            if(!IsValidBuilding(buildingId)) return StatusCode(404);

            var building = _buildingService.GetByOwnerAndId(User.FindFirstValue(ClaimTypes.NameIdentifier), buildingId);
            var currentBill = _billService.GetCurrentByBuilding(building);
            if(currentBill != null) ViewBag.InvalidAdd = true;
            else
            {
                var newBill = new Bill{
                    AddedAtUtc = DateTime.Now,
                    Id = Guid.NewGuid()
                };
                
                _billService.SaveAsync(newBill);
                building.Bills.Add(newBill.Id);
                _buildingService.UpdateAsync(building);

            }
            TempData["SelectedBuilding"] = building.Id;
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public IActionResult Add(){
            return View(nameof(Get));
        }

        public IActionResult Edit(Guid id){
            if(InvalidGuid(id)) return StatusCode(404);

            var building = _buildingService.GetByOwnerAndBillId(User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            if(building == null) return StatusCode(404);

            var bill = _billService.GetById(id);
            var a = _servicesService.GetAllByBuilding(building);
            ViewBag.Services = a;
            ViewBag.SelectedService = a.FirstOrDefault().Id.ToString();
            return View(bill);
        }
    }
}