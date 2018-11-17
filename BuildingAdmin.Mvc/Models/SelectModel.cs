using System;
using System.Collections.Generic;

namespace BuildingAdmin.Mvc.Models
{
    public class SelectModel
    {
        public string Id { get; set; }

        public List<IdName> Options { get; set; } = new List<IdName>();
        public string Name { get; set;}

        public SelectModel(){}
        public SelectModel(IEnumerable<dynamic> list)
        {
            foreach(var field in list){
                var sel = new IdName{
                    Id = field.Id.ToString(),
                    Name = field.Name
                };
                Options.Add(sel);
            }
        }
    }
    public class IdName {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}