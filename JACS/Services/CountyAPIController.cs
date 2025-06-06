using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CountyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage DeleteCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                ctl.DeleteCounty(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                County county = ctl.GetCounty(p1);
                return Request.CreateResponse(new CountyResult { data = county, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CountyResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                ctl.CreateCounty(county);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                ctl.UpdateCounty(county);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal class CountySearchResult
        {
            public List<CountyViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CountyResult
        {
            public County data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "name";
                    break;
                case 3:
                    fieldName = "code";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}