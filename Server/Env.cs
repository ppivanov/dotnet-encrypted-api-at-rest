using dotenv.net;
using System.Runtime.CompilerServices;

namespace EncryptedDbAtRest.Server;

public static class Env
{
    public static byte[]? EncryptionKey { get; private set; }

    public static byte[]? EncryptionIV { get; private set; }
    public static string DbString => Vars[EnvironmentVariableNames.DB_CONNECTION_STRING];

    public static bool IsDevelopment => Vars[EnvironmentVariableNames.IS_DEVELOPMENT] == "true";

    private static IDictionary<string, string>? VarsInternal { get; set; }

    public static IDictionary<string, string> Vars => VarsInternal!;

    public static void Initialize()
    {
        VarsInternal ??= DotEnv.Read();
        Vars.TryGetValue(EnvironmentVariableNames.DATA_ENCRYPTION_KEY_PATH, out string? keyPath);
        if (keyPath is null or "")
        {
            SetVars(new(EnvironmentVariableNames.DATA_ENCRYPTION_KEY_PATH, DefaultEncryptionKeyPath));
        }
        Thread.Sleep(1000);
        EncryptionKey = GetBytesFromFile(keyPath);

        Vars.TryGetValue(EnvironmentVariableNames.DATA_ENCRYPTION_IV_PATH, out string? ivPath);
        if (ivPath is null or "")
        {
            SetVars(new(EnvironmentVariableNames.DATA_ENCRYPTION_IV_PATH, DefaultEncryptionIVPath));
        }
        Thread.Sleep(1000);
        EncryptionIV = GetBytesFromFile(ivPath);
    }

    public static void SetVars(KeyValuePair<string, string> keyValuePair)
    {
        const string path = ".env";
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }
        var savedEnvVariables = File.ReadAllLines(path).ToList();

        var found = false;
        for (var i = 0; i < savedEnvVariables.Count; i++)
        {
            if (savedEnvVariables[i].Equals($"{keyValuePair.Key}=")) // if empty, add the value, otherwise ignore
            {
                found = true;
                savedEnvVariables[i] = $"{keyValuePair.Key}=\"{keyValuePair.Value}\"";
                Vars.Remove(keyValuePair.Key);
                Vars.Add(keyValuePair.Key, keyValuePair.Value);                               // replace the value in the dictionary

            }
            else if (savedEnvVariables[i].StartsWith($"{keyValuePair.Key}="))
            {
                found = true; // not modified
            }
        }
        if (!found) // if not found, add a new line
        {
            savedEnvVariables.Add($"{keyValuePair.Key}=\"{keyValuePair.Value}\"");   // add to file
            Vars.Add(keyValuePair.Key, keyValuePair.Value);                               // add to in memory dictionary
        }

        File.WriteAllLines(path, savedEnvVariables);
    }

    public static byte[]? GetBytesFromFile(string? path)
    {
        if (path != null && File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }

        return null;
    }

    /// <summary>
    /// Write bytes directly to a file, create the file if it doesn't exist yet.
    /// </summary>
    /// <param name="type">One of the <see cref="EnvironmentVariableNames"/></param>
    public static void WriteBytesToFilepathVariable(string variableName, byte[] bytes, [CallerFilePath] string actionFilePath = "", [CallerMemberName] string actionName = "")
    {
        Vars.TryGetValue(variableName, out string? path);
        if (path == null)
        {
            throw new Exception($"The environment variable for the given variable name is not set: [{variableName}]");
        }

        switch (variableName)
        {
            case EnvironmentVariableNames.DATA_ENCRYPTION_KEY_PATH:
                EncryptionKey = bytes;
                File.WriteAllBytes(path, bytes);
                break;
            case EnvironmentVariableNames.DATA_ENCRYPTION_IV_PATH:
                EncryptionIV = bytes;
                File.WriteAllBytes(path, bytes);
                break;
            default:
                break;
        }
    }

    public static string DefaultEncryptionKeyPath => "encryption.key";
    public static string DefaultEncryptionIVPath => "encryption.iv";
}

public static class EnvironmentVariableNames
{
    public const string DATA_ENCRYPTION_KEY_PATH = "DATA_ENCRYPTION_KEY_PATH";
    public const string DATA_ENCRYPTION_IV_PATH = "DATA_ENCRYPTION_IV_PATH";
    public const string DB_CONNECTION_STRING = "DB_CONNECTION_STRING";
    public const string IS_DEVELOPMENT = "IS_DEVELOPMENT";
}