/*
' Copyright (c) 2017 12th Judicial Circuit
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
using System.Web.Caching;

namespace tjc.Modules.PressCart.Components
{
    [TableName("tjc_presscart_divisions")]
    //setup the primary key for table
    [PrimaryKey("DivisionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Divisions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    public class Division
    {
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public string Judge { get; set; }
        public string County { get; set; }
        public bool IsCircuit { get; set; }
        [IgnoreColumn]
        public string DivisionJudge
        {
            get
            {
                if (!string.IsNullOrEmpty(DivisionName) && !string.IsNullOrEmpty(Judge))
                    return string.Format("{0} ({1})", DivisionName, Judge);
                return DivisionName;
            }
        }
    }
}
