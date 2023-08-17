/*
' Copyright (c) 2023  Joe Terhune
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

namespace tjc.Modules.FamilySelfHelp
{
    public class FamilySelfHelpModuleBase : PortalModuleBase
    {
        public long ClientId
        {
            get
            {
                var qs = Request.QueryString["cid"];
                if (qs != null)
                    return Convert.ToInt64(qs);
                return -1;
            }

        }
        public long LogId
        {
            get
            {
                var qs = Request.QueryString["lid"];
                if (qs != null)
                    return Convert.ToInt64(qs);
                return -1;
            }

        }
    }
}