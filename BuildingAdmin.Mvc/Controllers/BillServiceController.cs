using System;
using BuildingAdmin.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BuildingAdmin.Mvc.Controllers
{
    public class BillServiceController : BaseController
    {
        private readonly IBillService _billService;
        private readonly IBuildingService _buildingService;
        private readonly IServicesService _servicesService;

        public BillServiceController(IBillService billService, IBuildingService buildingService, IServicesService servicesService) : base(buildingService)
        {
            _billService = billService;
            _buildingService = buildingService;
            _servicesService = servicesService;
        }

        public IActionResult Add(Guid billId, Guid id){
            
            if(InvalidGuid(id) || InvalidGuid(billId)) return StatusCode(404);

            return View();
        }
    }
}