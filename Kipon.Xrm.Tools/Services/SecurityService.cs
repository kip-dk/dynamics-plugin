using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.ISecurityService))]
    public class SecurityService : ServiceAPI.ISecurityService
    {
        private string EQ_KEY = null;

        private readonly byte[] VALUES = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        [ImportingConstructor]
        public SecurityService()
        {
            var name = System.Environment.UserName + System.Environment.MachineName;

            for (var i = 0; i < 13 && i < name.Length; i++)
            {
                VALUES[i] = (byte)name[i];
            }
        }

        public string Decrypt(string pwd, string cipherText)
        {
            this.EQ_KEY = pwd;

            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EQ_KEY, VALUES);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string Encrypt(string pwd, string clearText)
        {
            this.EQ_KEY = pwd;

            if (string.IsNullOrEmpty(clearText)) return clearText;

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EQ_KEY, VALUES);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }
}
