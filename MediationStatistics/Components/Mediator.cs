/*
' Copyright (c) 2023 12th Judicial Circuit
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
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_mediators")]
    //setup the primary key for table
    [PrimaryKey("MediatorId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Mediators", CacheItemPriority.Default, 20)]
    internal class Mediator : EntityBase
    {
        public int MediatorId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        [IgnoreColumn]
        public string MediatorName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) & string.IsNullOrEmpty(LastName)) { return ""; }
                if (string.IsNullOrEmpty(FirstName) & !string.IsNullOrEmpty(LastName))
                    return LastName;
                if (!string.IsNullOrEmpty(FirstName) & string.IsNullOrEmpty(LastName))
                    return FirstName;

                return string.Format("{1}, {0}", FirstName, LastName);
            }
        }
    }
}
