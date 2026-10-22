using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Security.Roles;
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
    public class EmployeeController : DnnApiController
    {
        private readonly IHostSettings _hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetEmployeeDropDown(int employeeType)
        {
            IEnumerable<DropDownViewModel> empDropdownItem = Enumerable.Empty<DropDownViewModel>();
            try
            {
                var ctl = new Components.EmployeeController(_hostSettings);
                EmployeeTypes employeeTypeValue=(EmployeeTypes)employeeType;
                empDropdownItem = ctl.GetEmployeeDropDownByType(employeeTypeValue);
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
        public HttpResponseMessage GetCourtReporterDropDown(string roleName)
        {
            IEnumerable<DropDownViewModel> empDropdownItem = Enumerable.Empty<DropDownViewModel>();
            try
            {
                var ctl = new Components.EmployeeController(_hostSettings);
               var users= RoleController.Instance.GetUsersByRole(PortalSettings.PortalId, roleName);
                List<DropDownViewModel> reporters= new List<DropDownViewModel>();
                foreach (var user in users) { 
                    DropDownViewModel reporter= new DropDownViewModel { Id=user.UserID, Name=string.Format("{0}, {1}",user.LastName,user.FirstName)};
                    reporters.Add(reporter);
                }
                return Request.CreateResponse(new DropDownResult { data = reporters.OrderBy(x=>x.Name), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new DropDownResult { data = empDropdownItem, error = ex.Message });
            }
        }
        public class DropDownResult
        {
            public IEnumerable<DropDownViewModel> data { get; set; }
            public string error { get; set; }
        }
    }
}
