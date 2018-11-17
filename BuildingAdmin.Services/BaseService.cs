using System;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Services.Contracts;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.Services
{
    public class BaseService<T> : IBaseService<T> where T: Document
    {
        private readonly IBaseMongoRepository _repository;

        public BaseService(IBaseMongoRepository repository)
        {
            _repository = repository;
        }
        public async Task<Guid> SaveAsync(T model){
            await _repository.AddOneAsync<T>(model);
            return model.Id;
        }

        public async Task UpdateAsync(T model){
            await _repository.UpdateOneAsync<T>(model);
        }
    }
}