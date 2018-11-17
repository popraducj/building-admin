namespace BuildingAdmin.BussinessLogic.Cryptography
{
    public interface IArgonHash
    {
        EncodingResult Encoder(string plainText, HashingConfig config);
        EncodingResult Encoder(string plainText, HashingConfig config, string stringSalt);
    }
}