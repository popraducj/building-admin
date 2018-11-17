using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Services.Contracts
{
    public interface IApartmentService : IBaseService<Apartment>
    {
        List<Apartment> GetAllFromBuilding(Building building);
        Apartment GetById(Guid id);
    }
}