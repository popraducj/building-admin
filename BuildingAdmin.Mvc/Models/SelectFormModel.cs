using System.Collections.Generic;

namespace BuildingAdmin.Mvc.Models
{
    public class SelectFormModel : SelectModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string SubmitValue { get; set; }
        public SelectFormModel(): base(){}
        public SelectFormModel(IEnumerable<dynamic> list) : base(list) {}
        
    }
}