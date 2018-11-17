using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BuildingAdmin.DataLayer.Models
{
    public class History{

        [BsonElement]
        public string Value { get; set; }

        [BsonElement]
        public DateTime StartDate { get; set; }
        
        [BsonIgnoreIfNull]
        [BsonElement]
        public DateTime? EndDate { get; set; }
    }
}