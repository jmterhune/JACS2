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

using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.CourtRegistry.Components
{
    [TableName("tjc_car_attorneys")]
    //setup the primary key for table
    [PrimaryKey("AttorneyID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class Attorney
    {
        public int AttorneyID { get; set; }
        public int BarNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Language { get; set; }
        public string LawFirm { get; set; }
        public int UserID { get; set; }
    }
    internal class RegistryListItem:Attorney 
    {
        public int JacCodeID { get; set; }
        public int LocationID { get; set; }
        public int ApplicationID { get; set; }
        public int Status { get; set; }
    }
}
