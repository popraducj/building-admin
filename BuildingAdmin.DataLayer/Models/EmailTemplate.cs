using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    [CollectionName("templates")]    
    public class EmailTemplate: Document
    {
        [BsonElement]
        public EmailTypeEnum EmailType { get; set; }
        [BsonElement]
        public string Language {get;set;}
        [BsonElement]
        public string Subject {get;set;}
        [BsonElement]
        public string Template {get;set;}
        [BsonElement]
        public string Address {get;set;}
    }
}