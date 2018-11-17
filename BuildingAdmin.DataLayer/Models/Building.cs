using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    public class Building : Document{

        [BsonElement]
        public string Owner { get; set; }

        [BsonElement]
        public string Address { get; set; }

        [BsonElement]
        public string Name { get; set;}

        [BsonElement]
        public List<Guid> Services { get; set; } = new List<Guid>();

        [BsonElement]
        public List<Guid> Bills { get; set; } = new List<Guid>();

        [BsonElement]
        public string CUI { get; set; }

        [BsonElement]
        public List<Guid> Apartments { get; set; } = new List<Guid>();

        [BsonElement]
        public float Penalities { get; set; }
        [BsonElement]
        public int Order { get; set; }

        [BsonElement]
        public string IBAN { get; set; }

        public List<ApartmentSpecificBills> ApartmentSpecificBills { get; set; }

        [BsonElement]
        public bool IsActive { get; set; }
    }
}
