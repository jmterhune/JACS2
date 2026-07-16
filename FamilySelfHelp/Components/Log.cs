/*
' Copyright (c) 2023 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web.Services.Description;

namespace tjc.Modules.FamilySelfHelp.Components
{
    [TableName("tjc_shc_log")]
    //setup the primary key for table
    [PrimaryKey("LogId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("LogItems", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Log : EntityBase
    {
        public long LogId // bigint
        {
            get; set;
        }
        public long ClientId // int
        {
            get; set;
        }

        public string ClientType // nvarchar(20)
        {
            get; set;
        }
        public bool? HasAppointment // bit
        {
            get; set;
        }
        public DateTime? ServiceDate // datetime
        {
            get; set;
        }

        public bool IsNewCase // bit
        {
            get; set;
        }

        public string CaseNumber // nvarchar(250)
        {
            get; set;
        }

        public string Division // nvarchar(50)
        {
            get; set;
        }

        public string ContactMethod // nvarchar(50)
        {
            get; set;
        }

        public bool InterpreterProvided // bit
        {
            get; set;
        }
        [IgnoreColumn]
        public IEnumerable<CaseType> CaseTypes
        {
            get
            {
                var ctl = new Components.LogController();
                return ctl.GetCaseTypesByLog(LogId);
            }
        }
        [IgnoreColumn]
        public IEnumerable<Service> Services
        {
            get
            {
                var ctl = new Components.LogController();
                return ctl.GetServicesByLog(LogId);
            }
        }
        public string Location // nvarchar(50)
        {
            get; set;
        }

        public decimal TimeSpent // decimal(18,2)
        {
            get; set;
        }
        [IgnoreColumn]
        public string FormattedServiceProvided
        {
            get
            {
               var ctl=new Components.LogController();
                var services=ctl.GetServicesByLog(LogId).Select(x=>x.ServiceName);
                return string.Join(", ", services);
            }
        }
        [IgnoreColumn]
        public string FormattedCaseType
        {
            get
            {
                var ctl = new Components.LogController();
                var caseTypes = ctl.GetCaseTypesByLog(LogId).Select(x => x.CaseTypeName);
                return string.Join(", ", caseTypes);
            }
        }
        [IgnoreColumn]
        public Client ClientInfo
        {
            get
            {
                Client client = new Client();
                var ctl = new ClientController();
                client = ctl.GetClient(ClientId);
                if (client != null) { return client; } else { return new Client(); }
            }
        }
    }
    [TableName("tjc_shc_case_types")]
    internal class CaseType
    {
        public long LogID { get; set; } // bigint
        public string CaseTypeName { get; set; } // nvarchar(50)
    }
    [TableName("tjc_shc_services")]
    internal class Service
    {
        public long LogID { get; set; } // bigint
        public string ServiceName { get; set; } // nvarchar(50)

    }
    internal class Report : Log
    {
        public string LastName
        {
            get; set;
        }
        public string Name { get; set; }
        public string FirstName
        {
            get; set;
        }
        public string MiddleInitial
        {
            get; set;
        }

        [IgnoreColumn]
        public string FullName
        {
            get
            {
                if (MiddleInitial != "")
                    return string.Format("{0}, {1}&nbsp;{2}", LastName, FirstName, MiddleInitial);
                else
                    return string.Format("{0}, {1}", LastName, FirstName);
            }
        }
    }
}
