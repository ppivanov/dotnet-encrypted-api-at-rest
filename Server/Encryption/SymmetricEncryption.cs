using System.Security.Cryptography;
using System.Text;

namespace EncryptedDbAtRest.Server.Encryption;

/// <summary>
/// Singleton
/// </summary>
public sealed class SymmetricEncryption
{
    private readonly Aes _aes;
    public static SymmetricEncryption Instance { get; private set; } = null!;
    public static Encoding Encoding => Encoding.Unicode;

    private SymmetricEncryption()
    {
        _aes = Aes.Create();
        SetSecrets();

        _aes.Padding = PaddingMode.Zeros;
    }

    private void SetSecrets()
    {
        if (Env.EncryptionKey?.Length > 0)
        {
            _aes.Key = Env.EncryptionKey;
        }
        else
        {
            _aes.GenerateKey();
            Env.WriteBytesToFilepathVariable(EnvironmentVariableNames.DATA_ENCRYPTION_KEY_PATH, _aes.Key);
        }

        if (Env.EncryptionIV?.Length > 0)
        {
            _aes.IV = Env.EncryptionIV;
        }
        else
        {
            _aes.GenerateIV();
            Env.WriteBytesToFilepathVariable(EnvironmentVariableNames.DATA_ENCRYPTION_IV_PATH, _aes.IV);
        }
    }

    /// <summary>
    /// Exists so that the singleton can be initalized after the Env variables and keys are all loaded
    /// </summary>
    public static void Initialize()
    {
        Instance = new();
    }

    public string Encrypt(ref string str)
    {

        //var inBytes = Encoding.GetBytes(str);
        //var xfrm = _aes.CreateEncryptor();
        //var outBytes = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);

        //return Encoding.GetString(outBytes) ?? throw new Exception("Failed to encrypt data");

        // Create an encryptor to perform the stream transform.
        var encryptor = _aes.CreateEncryptor();

        // Create the streams used for encryption.
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(str);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(ref string cipherText)
    {
        //var inBytes = Encoding.GetBytes(cipherText);
        //var xfrm = _aes.CreateDecryptor();
        //var outBytes = xfrm.TransformFinalBlock(inBytes, 0, inBytes.Length);

        //return Encoding.GetString(outBytes);

        var decryptor = _aes.CreateDecryptor();

        // Create the streams used for decryption.
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}