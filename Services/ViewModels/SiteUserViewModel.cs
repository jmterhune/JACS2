using Newtonsoft.Json;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    internal class SiteUserViewModel
    {       
        public SiteUserViewModel(SiteUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserId = user.UserId;
        }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("userid")]
        public int UserId { get; set; }
    }
}