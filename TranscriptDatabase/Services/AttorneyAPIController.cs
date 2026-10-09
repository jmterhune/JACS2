using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;

namespace tjc.Modules.TranscriptDatabase.Services
{
    [DnnAuthorize]
    public class AttorneyController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDesignationAttorneys(int designationId)
        {
            IEnumerable<AttorneyViewModel> attorneys = Enumerable.Empty<AttorneyViewModel>();
            try
            {
                var ctl = new Components.AttorneyController(_hostSettings);
                attorneys = ctl.GetDesignationAttorneys(designationId);
                return Request.CreateResponse(new AttorneyResult { data = attorneys, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new AttorneyResult { data = attorneys, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAttorneyDropDown()
        {
            IEnumerable<DropDownViewModel> attorneys = Enumerable.Empty<DropDownViewModel>();
            try
            {
                var ctl = new Components.AttorneyController(_hostSettings);
                attorneys = ctl.GetAttorneyDropDownList();
                return Request.CreateResponse(new DropDownResult { data = attorneys, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new DropDownResult { data = attorneys, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetJudgeDropDown()
        {
            IEnumerable<DropDownViewModel> empDropdownItem = Enumerable.Empty<DropDownViewModel>();
            try
            {
                var ctl = new Components.EmployeeController(_hostSettings);
                empDropdownItem = ctl.GetEmployeeDropDownByType( EmployeeTypes.Judge);
                return Request.CreateResponse(new DropDownResult { data = empDropdownItem, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new DropDownResult { data = empDropdownItem, error = ex.Message });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [ActionName("RemoveAttorney")]
        public HttpResponseMessage DeleteDesignationAttorney(int designationId, int attorneyId)
        {
            try
            {
                var ctl = new DesignationController(_hostSettings);
                ctl.DeleteDesignationAttorney(designationId, attorneyId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("CreateAttorney")]
        public HttpResponseMessage CreateAttorney(AttorneyViewModel attorneyViewModel)
        {
            var ctl = new Components.AttorneyController(_hostSettings);
            Attorney attorney = new Attorney
            {
                Address2 = attorneyViewModel.Address2,
                Address1 = attorneyViewModel.Address1,
                City = attorneyViewModel.City,
                FirstName = attorneyViewModel.FirstName,
                LastName = attorneyViewModel.LastName,
                MiddleName = attorneyViewModel.MiddleName,
                OfficeID = attorneyViewModel.OfficeId,
                State = attorneyViewModel.State,
                ZipCode = attorneyViewModel.ZipCode,
                CreatedByUserID = attorneyViewModel.CreatedByUserID,
                CreatedDate = DateTime.Now,
                LastModifiedByUserID = attorneyViewModel.CreatedByUserID,
                LastModifiedDate = DateTime.Now
            };
            try
            {
                ctl.CreateAttorney(attorney);
                bool result = attorney.AttorneyID > 0;
                if (result)
                {
                    var dCtl = new DesignationController(_hostSettings);
                    attorneyViewModel.ListName = attorney.ListName;
                    attorneyViewModel.OfficeName = attorney.OfficeName;
                    attorneyViewModel.AttorneyId=attorney.AttorneyID;
                    return Request.CreateResponse(new AttorneyAddResult { data = attorneyViewModel });
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
        public class AttorneyResult
        {
            public IEnumerable<AttorneyViewModel> data { get; set; }
            public string error { get; set; }

        }
        public class AttorneyAddResult
        {
            public AttorneyViewModel data { get; set; }
        }
        public class DropDownResult
        {
            public IEnumerable<DropDownViewModel> data { get; set; }
            public string error { get; set; }
        }
    }
}
