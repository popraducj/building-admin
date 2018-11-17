using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingAdmin.DataLayer.Models
{
    public class BillServices
    {
        [BsonElement] public string Name { get; set; }
        [BsonElement] public decimal Price { get; set; }
        [BsonElement] public List<Guid> Apartmenets { get; set; }
    }
}