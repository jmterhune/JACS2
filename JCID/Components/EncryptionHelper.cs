using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotNetNuke.Common.Utilities;

namespace tjc.Modules.JCID.Components
{
    // SSNs are stored encrypted in the jud12 database. This intranet admin module decrypts them for display
    // and re-encrypts on save, so it must use the SAME key as the public JCID module: the site web.config
    // appSetting JCID_SSN_EncryptionKey (64 hex chars / 256 bits) must match the public site's value.
    public static class EncryptionHelper
    {
        private const string KeySettingName = "JCID_SSN_EncryptionKey"; // 32 bytes = 256 bits
        private static readonly byte[] Key;

        static EncryptionHelper()
        {
            string keyHex = Config.GetSetting(KeySettingName);
            if (string.IsNullOrEmpty(keyHex))
            {
                throw new InvalidOperationException($"Encryption key '{KeySettingName}' not found in web.config appSettings.");
            }

            // Expect 64-char hex string (32 bytes)
            if (keyHex.Length != 64)
            {
                throw new InvalidOperationException("Encryption key must be 64 hex characters (32 bytes / 256 bits).");
            }

            Key = HexStringToByteArray(keyHex);
        }

        public static string EncryptSSN(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Generate random IV per encryption
                aes.GenerateIV();
                byte[] iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);

                using (MemoryStream ms = new MemoryStream())
                {
                    // Prepend IV
                    ms.Write(iv, 0, iv.Length);

                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    byte[] encrypted = ms.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        public static string DecryptSSN(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Extract IV (first 16 bytes)
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                byte[] actualCipher = new byte[fullCipher.Length - 16];
                Array.Copy(fullCipher, 16, actualCipher, 0, actualCipher.Length);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);

                using (MemoryStream ms = new MemoryStream(actualCipher))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
