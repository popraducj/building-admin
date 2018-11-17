using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using MongoDbGenericRepository;

namespace BuildingAdmin.Services.Implementations
{
    public class BuildingService : BaseService<Building>, IBuildingService
    {
        private readonly IBaseMongoRepository _repository;

        public BuildingService(IBaseMongoRepository repository) : base(repository)
        {
            _repository = repository;
        }
        public List<Building> GetAllByOwner(string owner){
            return _repository.GetAll<Building>(x => x.Owner.Equals(owner) && x.IsActive) ?? new List<Building>();
        }
        public Building GetByOwnerAndApartmentId(string owner, Guid apartamentId)
        {
            return _repository.GetOne<Building>(x => x.Owner.Equals(owner) && x.Apartments.Contains(apartamentId) && x.IsActive);
        }

        public Building GetByOwnerAndBillId(string owner, Guid billId)
        {
            return _repository.GetOne<Building>(x => x.Owner.Equals(owner) && x.Bills.Contains(billId)&& x.IsActive);
        }

        public Building GetByOwnerAndId(string owner, Guid id){
            return _repository.GetOne<Building>(x=> x.Id.Equals(id) && x.Owner.Equals(owner) && x.IsActive);
        }

        public new async Task<Guid> SaveAsync(Building model){

            var existingServices =  _repository.GetAll<Service>(x => x.IsFromDefault && x.Owner.Equals(model.Owner));
            if (existingServices.Count != 0){
                return await SaveAsync(model, existingServices);
            }
            var defaultServices = _repository.GetAll<DefaultServices>(x => x.Language== "ro");
            var services = new List<Service>();
            foreach(var service in defaultServices){
                services.Add(new Service{
                    Name = service.Name,
                    Unit = service.Unit,
                    WithPersonalReading = service.WithPersonalReading,
                    DefaultValue = service.DefaultValue,
                    DisplayNameForApartments = service.DisplayNameForApartments,
                    Owner = model.Owner,
                    IsFromDefault = true
                });
            }
            _repository.AddMany(services);
            return await SaveAsync(model, services);
        }

        private async Task<Guid> SaveAsync(Building model, List<Service> services){
            foreach(var service in services){
                model.Services.Add(service.Id);
            }

            await _repository.AddOneAsync<Building>(model);
            return model.Id;
        }
    }
}