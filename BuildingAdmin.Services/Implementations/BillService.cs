using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using MongoDbGenericRepository;

namespace BuildingAdmin.Services.Implementations
{
    public class BillService : BaseService<Bill>, IBillService
    {
        private readonly IBaseMongoRepository _repository;

        public BillService(IBaseMongoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public List<Bill> GetAllFromBuilding(Building building){
            return _repository.GetAll<Bill>(x => building.Bills.Contains(x.Id)).OrderBy(x => x.AddedAtUtc).ToList();
        }

        public Bill GetCurrentByBuilding(Building building){
            return _repository.GetOne<Bill>(b => b.End == null && building.Bills.Contains(b.Id));
        }

        public Bill GetById(Guid id){

            return _repository.GetOne<Bill>(b => b.Id == id);
        }
    }
}