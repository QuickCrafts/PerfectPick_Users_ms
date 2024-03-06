using System.Security.Cryptography;
using System.Text;

namespace _PerfectPickUsers_MS.Functions
{
    public class AESModule
    {
        public readonly byte[] Key;
        public readonly byte[] IV;

        public AESModule()
        {
            try
            {
                string? tryKey = Environment.GetEnvironmentVariable("secretTokenKey");
                if (string.IsNullOrEmpty(tryKey))
                {
                    throw new Exception("Secret token key not found in enviornment");
                }
                Key = Encoding.ASCII.GetBytes(tryKey);
            }
            catch(Exception e)
            {
                throw new Exception("Error while creating AESModule: " + e.Message);
            }

            try
            {
                string? tryIV = Environment.GetEnvironmentVariable("secretTokenIV");
                if (string.IsNullOrEmpty(tryIV))
                {
                    throw new Exception("Secret key not found in enviornment");
                }
                IV = Encoding.ASCII.GetBytes(tryIV);
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating AESModule: " + e.Message);
            }
           
        }

        public string EncryptString(string plainText)
        {

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            string encryptedText = Convert.ToBase64String(encrypted);
            return encryptedText;
        }

        public int DecryptString(string cipherText)
        {
            byte[] decrypted = Convert.FromBase64String(cipherText);
            string? plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(decrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            int result = Convert.ToInt32(plaintext);
            return result;
        }
    }
}