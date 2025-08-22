using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class CourtTemplateOrderViewModel
    {
        public CourtTemplateOrderViewModel(CourtTemplateOrder courtTemplateOrder)
        {
            id = courtTemplateOrder.id;
            template_id = courtTemplateOrder.template_id.HasValue ? courtTemplateOrder.template_id : null;
            court_id = courtTemplateOrder.court_id;
            order = courtTemplateOrder.auto ? courtTemplateOrder.order.HasValue ? courtTemplateOrder.order : null : null;
            date = !courtTemplateOrder.auto ? courtTemplateOrder.date.HasValue ? courtTemplateOrder.date : null : null;
            date_string = courtTemplateOrder.date.HasValue ? courtTemplateOrder.date.Value.ToString("ddd, MMMM dd, yyyy") : null;
            auto = courtTemplateOrder.auto;
        }

        public CourtTemplateOrderViewModel() { }

        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("order")]
        public int? order { get; set; }
        [JsonProperty("court_id")]
        public long court_id { get; set; }
        [JsonProperty("date")]
        public DateTime? date { get; set; }
        [JsonProperty("date_string")]
        public string date_string { get; set; }
        [JsonProperty("template_id")]
        public long? template_id { get; set; }
        [JsonProperty("auto")]
        public bool auto { get; set; }

    }
}