/*
' Copyright (c) 2025 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.CourtReporting.Components
{
    [TableName("tjc_dcr_accounting")]
    //setup the primary key for table
    [PrimaryKey("AccountID", AutoIncrement = true)]
    //configure caching using PetaPoco
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Account
    {
        public int AccountID { get; set; }
        public int RequestId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Payment { get; set; }
        public string ReceivedBy { get; set; }
        public string CheckNumber { get; set; }
        public bool NFR { get; set; }
        public string Notes { get; set; }
    }
}
