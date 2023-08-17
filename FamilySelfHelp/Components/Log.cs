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
using System.Text.RegularExpressions;
using System.Web.Caching;

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
        public long LogId
        {
            get; set;
        }
        public long ClientId
        {
            get; set;
        }

        public string ClientType
        {
            get; set;
        }
        public bool? HasAppointment
        {
            get; set;
        }
        public DateTime? ServiceDate
        {
            get; set;
        }

        public bool IsNewCase
        {
            get; set;
        }

        public string CaseNumber
        {
            get; set;
        }

        public string Division
        {
            get; set;
        }

        public string ContactMethod
        {
            get; set;
        }

        public bool InterpreterProvided
        {
            get; set;
        }

        public string CaseType
        {
            get; set;
        }

        public string ServiceProvided
        {
            get; set;
        }


        public string Location
        {
            get; set;
        }

        public decimal TimeSpent
        {
            get; set;
        }
        [IgnoreColumn]
        public string FormattedServiceProvided
        {
            get
            {
                string[] services = ServiceProvided.Split('|');
                for (int i = 0; i < services.Length; i++)
                {
                    if (!services[i].Contains(" "))
                    {
                        services[i] = Regex.Replace(services[i], @"(?<!_)([A-Z])", " $1").Trim();
                    }
                }

                return string.Join(", ", services);
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
}
