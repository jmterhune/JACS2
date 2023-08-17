/*
' Copyright (c) 2020  Joe Terhune
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

namespace tjc.Modules.ZoomConnector
{
    public class ZoomConnectorModuleBase : PortalModuleBase
    {
        public bool Success
        {
            get
            {
                bool success = false;
                var qs = Request.QueryString["s"];
                if (qs != null)
                    if (qs.ToString() == "1")
                    {
                        success = true;
                    };
                return success;
            }

        }
        public string ManateeConnectorIP
        {
            get
            {
                if (Settings.Contains("ConnectorIP"))
                    return Settings["ConnectorIP"].ToString();
                return "";
            }
        }
        public string SarasotaConnectorIP
        {
            get
            {
                if (Settings.Contains("SarasotaConnectorIP"))
                    return Settings["SarasotaConnectorIP"].ToString();
                return "";
            }
        }
        public string DeSotoConnectorIP
        {
            get
            {
                if (Settings.Contains("DeSotoConnectorIP"))
                    return Settings["DeSotoConnectorIP"].ToString();
                return "";
            }
        }

    }
}