using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
     [CollectionName("bills")]
    public class Bill : Document{

        [BsonElement] public DateTime? End { get; set; }
        [BsonElement] public DateTime? DueDate { get; set; }
        [BsonElement] public List<BillServices> BillServices { get; set; }
        [BsonElement] public bool IsActive { get; set; }
    }
}
