using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    [BsonIgnoreExtraElements]
    public class UserProfile : Document
    {
        [BsonElement]
        public string Email {get;set;}
        [BsonElement]
        public string FirstName {get;set;}

        [BsonElement]
        public string LastName {get;set;}

        [BsonElement]
        public string Description {get;set;}

        [BsonElement]
        public string Address {get;set;}
        [BsonElement]
        public string Phone {get;set;}
        
        [BsonElement]
        public List<string> Roles {get;set;}
    }
}