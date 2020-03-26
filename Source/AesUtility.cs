using System.IO;
using System.Security.Cryptography;

namespace CloudUn
{
    public class AesUtility
    {
        static byte[] EncryptString(string plain, byte[] key, byte[] IV)
        {
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using (var writer = new StreamWriter(cs))
                {
                    //Write all data to the stream.
                    writer.Write(plain);
                }
                encrypted = ms.ToArray();
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptString(byte[] encrypted, byte[] key, byte[] IV)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using var ms = new MemoryStream(encrypted);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using (var reader = new StreamReader(cs))
                {
                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    plaintext = reader.ReadToEnd();
                }
            }

            return plaintext;
        }
    }
}