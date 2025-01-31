/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using System;

namespace tjc.Modules.CourtRegistry
{
    public class CourtRegistryModuleBase : PortalModuleBase
    {
        public int RequestedYear
        {
            get
            {
                var qs = Request.QueryString["yr"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int RequestedLocationId
        {
            get
            {
                var qs = Request.QueryString["loc"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int _year { get; set; }
        public int _locationId { get; set; }
        public string LocationName
        {
            get
            {
                if (ViewState["LocationName"] != null)
                    return ViewState["LocationName"].ToString();
                else
                    return "";
            }
            set
            {
                ViewState["LocationName"] = value;
            }
        }

    }
}