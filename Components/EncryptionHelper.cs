using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotNetNuke.Services.Exceptions;

namespace tjc.Modules.jacs.Components
{
    public static class EncryptionHelper
    {
        private static readonly byte[] Key;
        private static readonly byte[] IV;

        static EncryptionHelper()
        {
            try
            {
                string keyString = ConfigurationManager.AppSettings["JACS.Aes.Key"];
                string ivString = ConfigurationManager.AppSettings["JACS.Aes.IV"];

                if (string.IsNullOrWhiteSpace(keyString))
                    throw new ConfigurationErrorsException("Missing required appSetting: JACS.Aes.Key");

                if (string.IsNullOrWhiteSpace(ivString))
                    throw new ConfigurationErrorsException("Missing required appSetting: JACS.Aes.IV");

                Key = Encoding.UTF8.GetBytes(keyString);
                IV = Encoding.UTF8.GetBytes(ivString);

                if (Key.Length != 32)
                    throw new ConfigurationErrorsException($"AES Key must be exactly 32 bytes. Actual length: {Key.Length}");

                if (IV.Length != 16)
                    throw new ConfigurationErrorsException($"AES IV must be exactly 16 bytes. Actual length: {IV.Length}");
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                throw;
            }
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(buffer))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                // Safe fallback — return original text so one bad record doesn't crash the whole page
                return cipherText;
            }
        }
    }
}