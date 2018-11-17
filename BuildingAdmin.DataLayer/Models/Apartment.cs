using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;

namespace BuildingAdmin.DataLayer.Models
{
    public class Apartment : Document{

        [BsonElement]
        public int Number { get; set; }

        //cota parte
        [BsonElement]
        public float Cut { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement]
        public string OwnerEmail { get; set; }
        [BsonIgnoreIfNull]
        [BsonElement]
        public string OwnerPhone { get; set; }
        
        [BsonElement]
        public List<History> NoOfPersons { get; set; }

        [BsonElement]
        public List<History> FirstName { get; set; }

        [BsonElement]
        public byte Entrance { get; set; }
        [BsonElement]
        public List<History> LastName { get; set; }

        public List<ApartmentSpecificBills> SpecificBills { get; set; }

        public void AddHistoryNewItems(string firstName, string lastName, int noOfPersons){
            FirstName = new List<History>{
                new History{
                    Value = firstName,
                    StartDate = DateTime.Now
                }
            };
            LastName = new List<History>{
                new History{
                    Value = lastName,
                    StartDate = DateTime.Now
                }
            };
            NoOfPersons = new List<History>{
                new History{
                    Value = noOfPersons.ToString(),
                    StartDate = DateTime.Now
                }
            };
        }

        public void AddToHistory(string firstName, string lastName, int noOfPersons){
            SetHistoryList(firstName, FirstName);
            SetHistoryList(lastName, LastName);
            SetHistoryList(noOfPersons.ToString(), NoOfPersons);
        }

        private void SetHistoryList(string value, List<History> list){
            if(!value.Equals(list.Last().Value))
            {
                var date = DateTime.Now;
                list.Last().EndDate= date;
                list.Add(new History{
                    Value = value,
                    StartDate = date
                });
            }
        }
    }
}