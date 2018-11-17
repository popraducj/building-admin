using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    public class DefaultServices : Document
    {
        [BsonElement]
        public string Name { get; set; }

        //masure unit eg nr persons, mc
        [BsonElement]
        public string Unit { get; set; }
        public bool WithPersonalReading { get; set; }
        [BsonIgnoreIfNull]        
        [BsonElement]
        public string DisplayNameForApartments { get; set; }
        [BsonElement]
        public float DefaultValue { get; set; }
        [BsonElement]
        public string Language { get; set; }
    }
}