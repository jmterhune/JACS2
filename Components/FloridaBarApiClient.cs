using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace tjc.Modules.jacs.Components
{
    internal static class FloridaBarApiClient
    {
        public static async Task<FloridaBarMember> FetchAsync(string barNumber)
        {
            string apiBase = ConfigurationManager.AppSettings["FloridaBarApiBaseUrl"];
            string token = ConfigurationManager.AppSettings["FloridaBarBearerToken"];
            if (string.IsNullOrEmpty(apiBase) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(barNumber))
            {
                return null;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept-Version", "2.0");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    HttpResponseMessage response = await client.GetAsync(apiBase + barNumber).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject root = JObject.Parse(json);
                    JToken attrs = root["data"]?["attributes"];
                    if (attrs == null)
                    {
                        return null;
                    }

                    string first = (string)attrs["first-name"] ?? string.Empty;
                    string altFirst = (string)attrs["alt-first-name"] ?? string.Empty;
                    string last = (string)attrs["last-name"] ?? string.Empty;
                    string display = string.IsNullOrEmpty(altFirst)
                        ? (first + " " + last).Trim()
                        : altFirst + " (" + first + ") " + last;

                    return new FloridaBarMember
                    {
                        BarNumber = (string)root["data"]?["id"],
                        FirstName = first,
                        LastName = last,
                        DisplayName = display,
                        Email = (string)attrs["email"],
                        Phone = (string)attrs["phone"],
                        Eligible = (bool?)attrs["eligible"] ?? false,
                        StatusLabel = (string)attrs["status-label"]
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }

    internal class FloridaBarMember
    {
        public string BarNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Eligible { get; set; }
        public string StatusLabel { get; set; }

        public bool IsInGoodStanding
        {
            get { return !string.IsNullOrEmpty(StatusLabel) && StatusLabel.IndexOf("Good Standing", StringComparison.OrdinalIgnoreCase) >= 0; }
        }
    }
}
