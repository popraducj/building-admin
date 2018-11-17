using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using MongoDbGenericRepository;

namespace BuildingAdmin.Services.Implementations
{
    public class ServicesService : BaseService<Service>, IServicesService
    {
        
        private readonly IBaseMongoRepository _repository;

        public ServicesService(IBaseMongoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public List<Service> GetAllByBuilding(Building building){
            return _repository.GetAll<Service>(x => building.Services.Contains(x.Id)).OrderBy(x => x.AddedAtUtc).ToList();
        }

        public List<Service> GetAllNotSetToBuilding(List<Guid> buildingSerives, string owner)
        {
            var allOwnerServices = _repository.GetAll<Service>(x => x.Owner.Equals(owner));
            return allOwnerServices.Where(x => buildingSerives.All(s => !s.Equals(x.Id))).ToList();
        }

        public Service GetById(Guid id)
        {
            return _repository.GetOne<Service>(x => x.Id.Equals(id));
        }
    }
}