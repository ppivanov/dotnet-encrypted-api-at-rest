using System.Text;
using dotenv.net;

namespace EncryptedDbAtRest.Server;

public static class Env
{
     public static byte[] EncryptionKey { get; private set; } = null!;

     public static byte[] EncryptionIV { get; private set; } = null!;
     
     public static string DbString => Vars["DB_CONNECTION_STRING"];
     
     public static bool IsDevelopment => Vars["IS_DEVELOPMENT"] == "true";
     
     private static IDictionary<string, string>? VarsInternal { get; set; }
     
     public static IDictionary<string, string> Vars => VarsInternal!;
     
     public static void Init()
     {
          VarsInternal ??= DotEnv.Read();
          
          EncryptionKey = Encoding.Unicode.GetBytes(Vars["DATA_ENCRYPTION_KEY"]);
          EncryptionIV = Encoding.Unicode.GetBytes(Vars["DATA_ENCRYPTION_IV"]);
          
          Environment.SetEnvironmentVariable("DATA_ENCRYPTION_KEY", "");
          Environment.SetEnvironmentVariable("DATA_ENCRYPTION_IV", "");
     }
}