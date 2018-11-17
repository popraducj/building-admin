using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Services.Contracts
{
     public interface IServicesService: IBaseService<Service>
     {
         List<Service> GetAllByBuilding(Building building);
         List<Service> GetAllNotSetToBuilding(List<Guid> buildingSerives, string owner);
         Service GetById(Guid id);
     }
}