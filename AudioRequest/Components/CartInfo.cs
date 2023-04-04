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
    [TableName("tjc_PressCart")]
    //setup the primary key for table
    [PrimaryKey("CartId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Carts", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    class CartInfo
    {
        public int CartId { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string CourtDate { get; set; }

        public string CourtTime { get; set; }

        public string County { get; set; }

        public string Courthouse { get; set; }

        public string Courtroom { get; set; }

        public string Judge { get; set; }

        public bool TelevisionCombo { get; set; }

        public bool Laptop { get; set; }

        public bool Projector { get; set; }

        public bool DVD { get; set; }

        public bool VCR { get; set; }

        public bool Cassette { get; set; }

        public bool DocumentCamera { get; set; }

        public bool XRay { get; set; }

        public string Comments { get; set; }

        public bool TrainingRequired { get; set; }

        public int ModuleId { get; set; }

        public DateTime CreatedOnDate { get; set; }
    }
}
