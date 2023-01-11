using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Project.Models
{
    public static class AESEncryption
    {
        public static string Encrypt(string plainText, string password)
        {
            // generate salt
            byte[] salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);

            // derive key and initialization vector from password and salt
            var key = new Rfc2898DeriveBytes(password, salt, 10000);
            var aes = Aes.Create();
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            // create a encryptor to perform the stream transform
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // create the streams used for encryption
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // write all data to the stream
                        swEncrypt.Write(plainText);
                    }

                    // return the encrypted bytes from the memory stream
                    return Convert.ToBase64String(salt) + Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }


        public static string Decrypt(string encryptedText, string password)
        {
            // extract salt and encrypted bytes from the encrypted text
            byte[] salt = Convert.FromBase64String(encryptedText.Substring(0, 24));
            byte[] cipherBytes = Convert.FromBase64String(encryptedText.Substring(24));

            // derive key and initialization vector from password and salt
            var key = new Rfc2898DeriveBytes(password, salt, 10000);
            var aes = Aes.Create();
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            // create a decryptor to perform the stream transform
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // create the streams used for decryption
            using (var msDecrypt = new MemoryStream(cipherBytes))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // read the decrypted bytes from the decrypting stream and return as a plaintext string
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }














    }

}
