using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace NaptienMaster
{
    public class AepCrypto
    {
        public string secretKey { get; set; }
        public AepCrypto(string secretKey)
        {
            this.secretKey = secretKey;
        }
        public string convertToBase64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public string decodeBase64(string text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public string Aesp256Encryption(string plainText, byte[] key,byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode= CipherMode.CBC;
                ICryptoTransform aesCrypt=aes.CreateEncryptor();
                using(MemoryStream memoryStream= new MemoryStream())
                {
                    using(CryptoStream cryptoStream=new CryptoStream(memoryStream,aesCrypt,CryptoStreamMode.Write))
                    {
                        using(StreamWriter ws=new StreamWriter(cryptoStream))
                        {
                            ws.Write(convertToBase64(plainText));
                        }
                     return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }
        public string Aesp256Decryption(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV= iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform aesCrypt =aes.CreateDecryptor();
                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using(CryptoStream cryptoStream=new CryptoStream(memoryStream,aesCrypt,CryptoStreamMode.Read))
                    {
                        using(StreamReader rs=new StreamReader(cryptoStream))
                        {
                            return rs.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
