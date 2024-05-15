/*
' Copyright (c) 2024  Joe Terhune
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

namespace tjc.Modules.Purchasing
{
    public class PurchasingModuleBase : PortalModuleBase
    {
        public int CurrentOrderId
        {
            get
            {
                string qs = Request.QueryString["oid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string FormType
        {
            get
            {
                string qs = Request.QueryString["form"];
                if (qs != null)
                    return qs.ToString();
                return "";
            }
        }
        public int CurrentItemId
        {
            get
            {
                string qs = Request.QueryString["id"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int CurrentFormId
        {
            get
            {
                string qs = Request.QueryString["id"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int SessionItemId
        {
            get
            {
                string qs = Request.QueryString["sid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string EmailList
        {
            get
            {
                if (Settings.Contains("Emails"))
                {
                    return Settings["Emails"].ToString();
                }
                return "";
            }

        }
        public string SoTargetFolder
        {
            get
            {
                if (Settings.Contains("SoFolderName"))
                {
                    return Settings["SoFolderName"].ToString();
                }
                return "Supply-Order-Attachments";
            }
        }
        public string FoTargetFolder
        {
            get
            {
                if (Settings.Contains("FoFolderName"))
                {
                    return Settings["FoFolderName"].ToString();
                }
                return "Form-Order-Attachments";
            }
        }
        public string CoTargetFolder
        {
            get
            {
                if (Settings.Contains("CoFolderName"))
                {
                    return Settings["CoFolderName"].ToString();
                }
                return "Custom-Stamp-Attachments";
            }
        }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                {
                    return Settings["AdminRole"].ToString();
                }
                return "";
            }

        }
    }
}