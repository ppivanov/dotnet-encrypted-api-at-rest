using System.Security.Cryptography;
using System.Text;

namespace EncryptedDbAtRest.Server.Encryption;

/// <summary>
/// Singleton
/// </summary>
public sealed class SymmetricEncryption
{
    private readonly Aes _aes;
    private static SymmetricEncryption? _instance = null;
    
    private SymmetricEncryption()
    {
        _aes = Aes.Create();
        _aes.Key = Env.EncryptionKey;
        _aes.Padding = PaddingMode.PKCS7;
    }

    public static SymmetricEncryption GetInstance()
    {
        return _instance ??= new();
    }

    public string Encrypt(ref string str)
    {
        
        var inBytes = Encoding.Unicode.GetBytes(str);
        var xfrm = _aes.CreateEncryptor();
        var outBytes = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);
    
        return Encoding.Unicode.GetString(outBytes) ?? throw new Exception("Failed to encrypt data");;;
    }

    public string Decrypt(ref string encryptedStr)
    {
        var inBytes = Encoding.Unicode.GetBytes(encryptedStr);
        var xfrm = _aes.CreateDecryptor();
        var outBytes = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);
    
        return Encoding.Unicode.GetString(outBytes);
    }
}