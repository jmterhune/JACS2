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

namespace tjc.Modules.AudioRequest.Components
{
    [TableName("aws_dcr_Proceeding")]
    //setup the primary key for table
    [PrimaryKey("ProceedingId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Proceedings", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    class ProceedingInfo
    {
        public int ModuleId { get; set; }

        public int ProceedingId { get; set; }

        public string Requestor { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public int RequestorId { get; set; }

        public DateTime RequestedDate { get; set; }

        public string CDPreference { get; set; }

        public string Jurisdiction { get; set; }

        public string CaseName { get; set; }

        public string Judge { get; set; }

        public string CaseNumber { get; set; }

        public string ProceedingDate { get; set; }

        public string ProceedingTime { get; set; }

        public string Location { get; set; }

        public string ProceedingType { get; set; }

        public string Involvement { get; set; }

        public string Instructions { get; set; }

        public string TranscriptionList { get; set; }

        public string DeliveryMethod { get; set; }

        public bool Agency { get; set; }

        public bool CA { get; set; }

        public bool Closed { get; set; }

        public bool Paid { get; set; }

        public bool IsInquiry { get; set; }

    }
}
