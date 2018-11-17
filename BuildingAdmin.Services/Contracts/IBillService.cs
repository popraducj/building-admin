using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Services.Contracts
{ 
    public interface IBillService: IBaseService<Bill>
    {
        List<Bill> GetAllFromBuilding(Building building);
        Bill GetCurrentByBuilding(Building building);
        Bill GetById(Guid id);
    }
}