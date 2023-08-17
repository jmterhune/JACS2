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
using System.Web.Caching;

namespace tjc.Modules.FamilySelfHelp.Components
{
    [TableName("tjc_shc_client")]
    //setup the primary key for table
    [PrimaryKey("ClientId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Clients", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Client : EntityBase
    {

        public long ClientId
        {
            get;set;
        }
        public string LastName
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Phone
        {
            get; set;
        }

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
                    return string.Format("{0}, {1}&nbsp;{2}", LastName ,FirstName, MiddleInitial);
                else
                    return string.Format("{0}, {1}", LastName ,FirstName);
            }
        }
    }

    public class ClientName
    {
        public string Text
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }
    }
}
