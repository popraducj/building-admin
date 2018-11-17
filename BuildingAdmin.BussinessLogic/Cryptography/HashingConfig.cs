using System;

namespace BuildingAdmin.BussinessLogic.Cryptography
{
    public class HashingConfig
    {
        public string KnownSecret {get;set;}
        public string AssociatedData {get;set;}
        public int DegreeOfParallelism {get;set;}
        public int MemorySize {get;set;}
        public int Iterations {get;set;}
    }
}