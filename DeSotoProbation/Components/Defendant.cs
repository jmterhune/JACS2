/*
' Copyright (c) 2024 Joe Terhune
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

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_defendants")]
    //setup the primary key for table
    [PrimaryKey("DefendantID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Defendants", CacheItemPriority.Default, 20)]
    internal class Defendant
    {
       public long DefendantID { get; set; }
        public int ProgramID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime ProgramStartDate { get; set; }
        public string CaseNumber { get; set; }
        public decimal StartingBalance { get; set; }
        public string Notes { get; set; }
        public DateTime DueDate { get; set; }
        public int AssignedUserID { get; set; }
        public bool SpainshSpeaking { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public int CreatedByID { get; set; }
        public int LastModifiedByID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
