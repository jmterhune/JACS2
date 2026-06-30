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

namespace tjc.Modules.AudioRequest.Components
{
    // NOTE: table tjc_PressCart was NOT found in prod (intranet.jud12.local); string column sizes below are unverified.
    //       This model/controller is not surfaced by any editable .ascx form in this module.
    [TableName("tjc_PressCart")]
    //setup the primary key for table
    [PrimaryKey("CartId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Carts", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    class CartInfo
    {
        public int CartId { get; set; }  // int (identity PK)

        public string LastName { get; set; }  // nvarchar(?) — table not found in prod

        public string FirstName { get; set; }  // nvarchar(?) — table not found in prod

        public string MiddleName { get; set; }  // nvarchar(?) — table not found in prod

        public string Email { get; set; }  // nvarchar(?) — table not found in prod

        public string Phone { get; set; }  // nvarchar(?) — table not found in prod

        public string ContactName { get; set; }  // nvarchar(?) — table not found in prod

        public string ContactEmail { get; set; }  // nvarchar(?) — table not found in prod

        public string ContactPhone { get; set; }  // nvarchar(?) — table not found in prod

        public string CourtDate { get; set; }  // nvarchar(?) — table not found in prod

        public string CourtTime { get; set; }  // nvarchar(?) — table not found in prod

        public string County { get; set; }  // nvarchar(?) — table not found in prod

        public string Courthouse { get; set; }  // nvarchar(?) — table not found in prod

        public string Courtroom { get; set; }  // nvarchar(?) — table not found in prod

        public string Judge { get; set; }  // nvarchar(?) — table not found in prod

        public bool TelevisionCombo { get; set; }  // bit

        public bool Laptop { get; set; }  // bit

        public bool Projector { get; set; }  // bit

        public bool DVD { get; set; }  // bit

        public bool VCR { get; set; }  // bit

        public bool Cassette { get; set; }  // bit

        public bool DocumentCamera { get; set; }  // bit

        public bool XRay { get; set; }  // bit

        public string Comments { get; set; }  // nvarchar(?) — table not found in prod

        public bool TrainingRequired { get; set; }  // bit

        public int ModuleId { get; set; }  // int

        public DateTime CreatedOnDate { get; set; }  // datetime
    }
}
