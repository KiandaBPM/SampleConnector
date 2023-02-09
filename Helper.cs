using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleConnector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Kianda
{
    public class Helper
    {
        internal static readonly string ClientSecret = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("SecretKey")) ? WebConfigurationManager.AppSettings.Get("SecretKey") : WebConfigurationManager.AppSettings.Get("SecretKey");

        public static List<JObject> GetSampleTree()
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            JObject tree = JObject.Parse(File.ReadAllText(basedir + @"\tree.json"));
            List<JObject> items = new List<JObject>();
            items.Add(tree);
            return items;
        }
        public static JObject GetSettings(ConnectorRequest request)
        {
            var settingsPlain = Helper.AESDecrypt(request.encryptedSettingsPropertyBag, request.iv);
            var settingsObj = JsonConvert.DeserializeObject<JObject>(settingsPlain);
            return settingsObj;
        }
        public static string AESEncrypt(string plainData,  out byte[] iv)
        {
            var _key = Convert.FromBase64String(ClientSecret);
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider() { Mode = CipherMode.CBC, KeySize = 256, Key = _key })
            {
                aes.GenerateIV(); //Generate a ramdom IV.
                iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {

                            using (StreamWriter sw = new StreamWriter(cs))
                                sw.Write(plainData);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }

            }
        }

        public static string AESDecrypt(string encryptedData, byte[] iv)
        {

            var buffer = Convert.FromBase64String(encryptedData);
            var _key = Convert.FromBase64String(ClientSecret);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider() { KeySize = 256, Mode = CipherMode.CBC, Key = _key, IV = iv })
            using (var decryptor = aes.CreateDecryptor())
            {
                using (MemoryStream ms = new MemoryStream(buffer))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sw = new StreamReader(cs))
                        return sw.ReadToEnd();
                }
            }

        }

        public static string HashWithHMACSHA256(string value)
        {
            using (var hash = new HMACSHA256(Convert.FromBase64String(ClientSecret)))
            {
                var hashedByte = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                var hashed = Convert.ToBase64String(hashedByte);
                return hashed;
            }
        }
    }
}