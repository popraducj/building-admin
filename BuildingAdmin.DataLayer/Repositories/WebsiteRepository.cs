using Microsoft.Extensions.Configuration;
using MongoDbGenericRepository;

namespace BuildingAdmin.DataLayer.Repository
{
    public class WebsiteRepository : BaseMongoRepository
    {
        public WebsiteRepository(IConfiguration configuration) : base(configuration.GetConnectionString("DefaultConnection"), "ba"){
            
        }
    }
}