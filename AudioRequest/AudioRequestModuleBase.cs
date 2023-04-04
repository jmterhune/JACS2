/*
' Copyright (c) 2017  12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

namespace tjc.Modules.AudioRequest
{
    public class AudioRequestModuleBase : PortalModuleBase
    {
        public int ItemId
        {
            get
            {
                var qs = Request.QueryString["tid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public string Casenumber
        {
            get
            {
                var qs = Request.QueryString["casenumber"];
                if (qs != null)
                    return qs.ToString();
                return "";
            }

        }
        public string Email
        {
            get
            {
                var qs = Request.QueryString["email"];
                if (qs != null)
                    return Server.UrlDecode(qs);
                return "";
            }

        }
        public bool IsInquiry
        {
            get
            {
                var qs = Request.QueryString["inquiry"];
                if (qs != null)
                    return true;
                return false;

            }
        }
    }
}