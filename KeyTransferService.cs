using System.Security.Cryptography;
using System.Text;

public static class KeyTransferService
{
    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 300_000;

    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("DVK2");
    private const byte Version = 1;

    public static string ExportKeyToBase64(string sourceKeyFilePath, string password)
    {
        if (!File.Exists(sourceKeyFilePath))
            throw new FileNotFoundException("File not found.", sourceKeyFilePath);

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        byte[] plainBytes = File.ReadAllBytes(sourceKeyFilePath);
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] tag = new byte[TagSize];
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] key = DeriveKey(password, salt);

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);

            bw.Write(Magic);                          // 4
            bw.Write(Version);                       // 1
            bw.Write(Path.GetFileName(sourceKeyFilePath)); // string

            bw.Write(salt.Length);
            bw.Write(salt);

            bw.Write(nonce.Length);
            bw.Write(nonce);

            bw.Write(tag.Length);
            bw.Write(tag);

            bw.Write(cipherBytes.Length);
            bw.Write(cipherBytes);

            bw.Flush();

            return Convert.ToBase64String(ms.ToArray());
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    public static string ImportKeyFromBase64(string base64Text, string password, string targetFolder, string targetFileName)
    {
        if (string.IsNullOrWhiteSpace(base64Text))
            throw new ArgumentException("Base64 text cannot be empty.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        if (string.IsNullOrWhiteSpace(targetFolder))
            throw new ArgumentException("Target folder cannot be empty.");

        if (string.IsNullOrWhiteSpace(targetFileName))
            throw new ArgumentException("File name cannot be empty.");

        byte[] allBytes;
        try
        {
            allBytes = Convert.FromBase64String(base64Text.Trim());
        }
        catch
        {
            throw new Exception("Invalid base64 string.");
        }

        using var ms = new MemoryStream(allBytes);
        using var br = new BinaryReader(ms, Encoding.UTF8, leaveOpen: true);

        byte[] magic = br.ReadBytes(4);
        if (magic.Length != 4 || !CryptographicOperations.FixedTimeEquals(magic, Magic))
            throw new InvalidDataException("Data invalid.");

        byte version = br.ReadByte();
        if (version != Version)
            throw new InvalidDataException("Data version not valid.");

        string originalFileName = br.ReadString();

        int saltLength = br.ReadInt32();
        byte[] salt = br.ReadBytes(saltLength);

        int nonceLength = br.ReadInt32();
        byte[] nonce = br.ReadBytes(nonceLength);

        int tagLength = br.ReadInt32();
        byte[] tag = br.ReadBytes(tagLength);

        int cipherLength = br.ReadInt32();
        byte[] cipherBytes = br.ReadBytes(cipherLength);

        byte[] key = DeriveKey(password, salt);
        byte[] plainBytes = new byte[cipherBytes.Length];

        try
        {
            using var aes = new AesGcm(key, tagLength);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            Directory.CreateDirectory(targetFolder);

            string safeFileName = MakeSafeFileName(targetFileName);
            string outputPath = Path.Combine(targetFolder, safeFileName);

            File.WriteAllBytes(outputPath, plainBytes);
            return outputPath;
        }
        catch (CryptographicException)
        {
            throw new Exception("Wrong password or data corrupted.");
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plainBytes);
        }
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }

    private static string MakeSafeFileName(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(c, '_');

        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "imported.key";

        return fileName;
    }

    public static bool IsEncryptedDevKitPayload(byte[] data)
    {
        if (data == null || data.Length < 4)
            return false;

        return data[0] == (byte)'D'
            && data[1] == (byte)'V'
            && data[2] == (byte)'K'
            && data[3] == (byte)'2';
    }
}