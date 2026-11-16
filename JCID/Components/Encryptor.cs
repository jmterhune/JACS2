using System;
using System.Web;

namespace tjc.Modules.JCID.Components
{
    // Builds the tamper-proof "eid" query-string value used in the reject-correction link that is emailed to
    // the applicant. The applicant opens that link on the public (jud12) JCID Form, which decodes it with the
    // identical tjc.Modules.Utilities.Encryptor. The key and algorithm here MUST match that class exactly, or
    // the public site will reject the id as an invalid TamperProofString.
    public class Encryptor
    {
        private readonly string TamperProofKey = GetKey("zmencrypt");

        public string QueryStringEncode(string value)
        {
            return HttpUtility.UrlEncode(TamperProofStringEncode(value, TamperProofKey));
        }

        public string QueryStringDecode(string value)
        {
            return TamperProofStringDecode(value, TamperProofKey);
        }

        private string TamperProofStringEncode(string value, string key)
        {
            System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)) + '-' + Convert.ToBase64String(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
        }

        private string TamperProofStringDecode(string value, string key)
        {
            System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));

            string dataValue;
            try
            {
                dataValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split('-')[0]));
                string storedHash = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split('-')[1]));
                string calcHash = System.Text.Encoding.UTF8.GetString(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataValue)));

                if (storedHash != calcHash)
                    // Data was corrupted
                    throw new ArgumentException("Hash value does not match");
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid TamperProofString");
            }

            return dataValue;
        }

        private static string GetKey(string key)
        {
            return key.Remove(0, key.Length / 2);
        }
    }
}
