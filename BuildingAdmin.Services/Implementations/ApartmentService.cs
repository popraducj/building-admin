using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using MongoDbGenericRepository;

namespace BuildingAdmin.Services.Implementations
{
    public class ApartmentService : BaseService<Apartment>, IApartmentService
    {
        private readonly IBaseMongoRepository _repository;

        public ApartmentService(IBaseMongoRepository repository) : base(repository)
        {
            _repository = repository;
        }
        public List<Apartment> GetAllFromBuilding(Building building){
            return _repository.GetAll<Apartment>(x => building.Apartments.Contains(x.Id)).OrderBy(x => x.Number).ToList();
        }

        public Apartment GetById(Guid id){
            return _repository.GetOne<Apartment>(x => x.Id.Equals(id));
        }
    }
}