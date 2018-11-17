using System;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.Services
{
    public interface IBaseService<T>
    {        
        Task<Guid> SaveAsync(T model);
        Task UpdateAsync(T model);
    }
}