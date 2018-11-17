using System;
using System.Text;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace BuildingAdmin.BussinessLogic.Cryptography
{
    public class ArgonHash : IArgonHash
    {
        private static readonly RandomNumberGenerator Rng =  System.Security.Cryptography.RandomNumberGenerator.Create();
        public EncodingResult Encoder(string plainText, HashingConfig config){
                        
            byte[] salt = new byte[32];
            Rng.GetBytes(salt);
            var stringSalt = Convert.ToBase64String(salt);
            return Encoder(plainText, config, stringSalt);
        }
        public EncodingResult Encoder(string plainText, HashingConfig config, string stringSalt){
            
            if(stringSalt == null){
                byte[] salt = new byte[32];
                Rng.GetBytes(salt);
                stringSalt = Convert.ToBase64String(salt);
            }
            
            var argon2 = SetHashObject(plainText, config);
            argon2.Salt = Encoding.ASCII.GetBytes(stringSalt);

            return  new EncodingResult() { Hash = Convert.ToBase64String(argon2.GetBytes(512)), Salt = stringSalt};
        }

        private Argon2d SetHashObject(string password, HashingConfig config){

            var bytePassword = Encoding.ASCII.GetBytes(password);
            byte[] userUuidBytes =  Encoding.ASCII.GetBytes(config.AssociatedData);
            byte[] byteSecret = Encoding.ASCII.GetBytes(config.KnownSecret);
            
            var argon2 = new Argon2d(bytePassword);
            argon2.DegreeOfParallelism = config.DegreeOfParallelism;
            argon2.MemorySize = config.MemorySize;
            argon2.Iterations = config.Iterations;
            argon2.AssociatedData = userUuidBytes;
            argon2.KnownSecret = byteSecret;
            
            return argon2;
        }
    }
}