using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace GOTEX.Core.Utilities
{
    public static class Utils
    {
        public static string Stringify(this object any) => JsonConvert.SerializeObject(any);
        public static string GetValue(this Dictionary<string, string> dic, string key)
            => dic.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
        public static object GetValue(this Dictionary<string, object> dic, string key)
            => dic.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
        public static TOut Parse<TOut>(this string any) =>
            JsonConvert.DeserializeObject<TOut>(any, new JsonSerializerSettings
            {
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
                },
                Converters = { new IsoDateTimeConverter() }
            });
        public static async Task<HttpResponseMessage> Send(string url, HttpRequestMessage message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls 
                                                   | SecurityProtocolType.Tls11 
                                                   | SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                return await client.SendAsync(message);
            }
        }
        public static string GenerateSHA512(this string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
        public static List<SelectListItem> GetSortedCountry(this List<Nationality> countries, string selectedCountry)
        {
            var countrylist = new List<SelectListItem>();
            foreach (var item in countries)
            {
                if (item.CountryName == selectedCountry)
                {
                    countrylist.Add(new SelectListItem
                    {
                        Text = item.CountryName,
                        Value = item.CountryCode,
                        Selected = true
                    });
                }
                else
                {
                    countrylist.Add(new SelectListItem
                    {
                        Text = item.CountryName,
                        Value = item.Id.ToString(),
                        Selected = false
                    });
                }
            }
            return countrylist;
        }
        public static string RefrenceCode()
        {
            //generate 12 digit numbers
            var bytes = new byte[8];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            ulong random = BitConverter.ToUInt64(bytes, 0) % 1000000000000;
            return $"{random:D12}";
        }
        public static void SendMail(Dictionary<string, string> mailsettings, string toEmail, string subject, string body, string bcc = null)
        {
            var credentials = new NetworkCredential(mailsettings.GetValue("UserName"), mailsettings.GetValue("Password"));
            var smtp = new SmtpClient(mailsettings.GetValue("Host"), int.Parse(mailsettings.GetValue("Port")))
            {
                EnableSsl = bool.Parse(mailsettings.GetValue("UseSsl")),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials,
            };


            var mail = new MailMessage {From = new MailAddress(mailsettings.GetValue("Sender"))};
            mail.To.Add(new MailAddress(toEmail));;

            if (!string.IsNullOrEmpty(bcc))
            {
                var copies = bcc.Split(',');
                foreach (var email in copies)
                    mail.Bcc.Add(new MailAddress(email));
            }
            
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                smtp.SendMailAsync(mail).Wait();
            }
            catch (Exception ex)
            {
                
            }
        }

        public static string ReadTextFile(string webrootpath, string filename)
        {
            var body = string.Empty;
            using (var sr = new StreamReader($"{webrootpath}\\App_Data\\Templates\\{filename}"))
            {
                body =  sr.ReadToEndAsync().Result;
            }
            return body;
        }
    }
}