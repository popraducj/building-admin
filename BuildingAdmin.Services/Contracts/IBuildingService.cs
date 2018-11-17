using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Services.Contracts
{
    public interface IBuildingService : IBaseService<Building>
    {
        List<Building> GetAllByOwner(string owner);
        Building GetByOwnerAndId(string owner, Guid id);
        Building GetByOwnerAndApartmentId(string owner, Guid apartamentId);
        Building GetByOwnerAndBillId(string owner, Guid billId);
    }
}