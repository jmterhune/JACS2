using Newtonsoft.Json;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtEventTypeViewModel
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public long event_type_id { get; set; }

        // Adding the missing FromCourtEventTypes method to fix CS0117
        public static IEnumerable<CourtEventTypeViewModel> FromCourtEventTypes(IEnumerable<CourtEventType> courtEventTypes)
        {
            var viewModels = new List<CourtEventTypeViewModel>();
            foreach (var eventType in courtEventTypes)
            {
                viewModels.Add(new CourtEventTypeViewModel
                {
                    id = eventType.id,
                    court_id = eventType.court_id,
                    event_type_id = eventType.event_type_id
                });
            }
            return viewModels;
        }
    }
}