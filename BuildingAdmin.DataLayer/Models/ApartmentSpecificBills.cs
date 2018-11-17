
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingAdmin.DataLayer.Models
{
    public class ApartmentSpecificBills {

        [BsonElement]
        public string Name { get; set; }

        //cota parte
        [BsonElement]
        public string Unit { get; set; }

        [BsonElement]
        public float Quantity { get; set; }

        public List<string> Services { get; set; }        
    }
}