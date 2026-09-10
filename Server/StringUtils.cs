using System.Security.Cryptography;
using System.Text;

namespace EncryptedDbAtRest.Server;

public static class StringUtils
{
    public static string GetNewId()
    {
        return Guid.NewGuid()
            .ToString()
            .Replace("-", "");
    }

    /***************************************************************************************
    *    Stolen from: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/
    *    On: 05 May 2021
    *
    ***************************************************************************************/
    public static string ComputeSha256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));                                            // ComputeHash - returns byte array  

        StringBuilder builder = new();                                                                              // Convert byte array to a string
        for (int i = 0; i < bytes.Length; i++)
            builder.Append(bytes[i].ToString("x2"));

        return builder.ToString();
    }
}
