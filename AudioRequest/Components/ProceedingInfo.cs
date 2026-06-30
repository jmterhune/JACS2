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
    // NOTE: prod (intranet.jud12.local) table is actually named tjc_dcr_proceeding; column sizes below were verified against it.
    [TableName("aws_dcr_Proceeding")]
    //setup the primary key for table
    [PrimaryKey("ProceedingId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Proceedings", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    [Scope("ModuleId")]
    class ProceedingInfo
    {
        public int ModuleId { get; set; }  // int

        public int ProceedingId { get; set; }  // int (ProceedingID, identity PK)

        public string Requestor { get; set; }  // nvarchar(50)

        public string Address { get; set; }  // nvarchar(100)

        public string City { get; set; }  // nvarchar(50)

        public string State { get; set; }  // nchar(2)

        public string Zip { get; set; }  // nvarchar(11)

        public string Phone { get; set; }  // nvarchar(50) — stored formatted, e.g. "(941) 555-1234" (mask NOT stripped)

        public string Fax { get; set; }  // nvarchar(50) — stored formatted, e.g. "(941) 555-1234" (mask NOT stripped)

        public string Email { get; set; }  // nvarchar(150)

        public int RequestorId { get; set; }  // int (RequestorID)

        public DateTime RequestedDate { get; set; }  // smalldatetime

        public string CDPreference { get; set; }  // nvarchar(25)

        public string Jurisdiction { get; set; }  // nvarchar(10)

        public string CaseName { get; set; }  // nvarchar(250)

        public string Judge { get; set; }  // nvarchar(50)

        public string CaseNumber { get; set; }  // nvarchar(20)

        public string ProceedingDate { get; set; }  // nvarchar(50)

        public string ProceedingTime { get; set; }  // nvarchar(100)

        public string Location { get; set; }  // nvarchar(10)

        public string ProceedingType { get; set; }  // nvarchar(250)

        public string Involvement { get; set; }  // nvarchar(50)

        public string Instructions { get; set; }  // nvarchar(750)

        public string TranscriptionList { get; set; }  // nvarchar(3)

        public string DeliveryMethod { get; set; }  // nvarchar(50)

        public bool Agency { get; set; }  // bit

        public bool CA { get; set; }  // bit

        public bool Closed { get; set; }  // bit

        public bool Paid { get; set; }  // bit

        public bool IsInquiry { get; set; }  // bit

    }
}
