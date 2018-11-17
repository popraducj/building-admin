using System;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace BuildingAdmin.DataLayer
{
    public class MongoInstance
    {
        protected MongoClient Client{get;set;}
        protected IMongoDatabase database;
        public MongoInstance(IConfiguration configuration, string db = "main")
        {
            Client = new MongoClient(configuration.GetConnectionString("DefaultConnection"));
            database = Client.GetDatabase(configuration["Database:"+db]);
        }
    }
}
