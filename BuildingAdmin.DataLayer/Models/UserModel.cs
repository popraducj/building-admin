using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    [CollectionName("users")]
    public class UserModel : IDocument<string>
    {
        [BsonId]
        public string Id { get; set; }
        [BsonElement]
        public int Version { get; set; }
        [BsonElement]
        public string Password { get; set; }
        [BsonElement]
        public string Salt { get; set; }

        [BsonElement]
        public UserStatesEnum? State { get; set; }

        [BsonElement]
        public string FirstName { get; set; }

        [BsonElement]
        public string LastName { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string Token { get; set; }

        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string Language { get; set; }
        
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public DateTime? ExpireDate { get; set; }

        public List<string> Roles { get; set; }

        [BsonElement]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string Description { get; set; }

        [BsonElement]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string Address { get; set; }
        [BsonElement]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}