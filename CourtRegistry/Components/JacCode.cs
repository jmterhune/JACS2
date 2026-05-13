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

    [TableName("tjc_car_jac_codes")]
    //setup the primary key for table
    [PrimaryKey("JacCodeID", AutoIncrement = false)]
    //configure caching using PetaPoco
    internal class JacCode
    {
        public int JacCodeID { get; set; }
        public string Category { get; set; }
        public string LabelNote { get; set; }
        public int CaseTypeID { get; set; }
        public bool Active { get; set; }
        [IgnoreColumn]
        public string JacCodeListName { get { return string.Format("{0} ({1} - {2})", JacCodeID, CaseTypeName, Category); } }
        [IgnoreColumn]
        public string CaseTypeName
        {
            get
            {
                var ctl = new CaseTypeController();
                CaseType caseType = ctl.GetCaseType(CaseTypeID);
                return caseType != null ? caseType.CaseTypeName : string.Empty;
            }
        }
    }
    [TableName("tjc_car_jac_codes_updates")]
    //setup the primary key for table
    [PrimaryKey("JacCodeID", AutoIncrement = false)]
    //configure caching using PetaPoco
    internal class JacCodeUpdate
    {
        public int JacCodeID { get; set; }
        public string Category { get; set; }
        public int CaseTypeID { get; set; }
        public int UpdateType { get; set; }
    }
    [TableName("tjc_car_jac_code_config")]
    internal class JacCodeConfig
    {
        public int JacCodeID { get; set; }
        public int LocationID { get; set; }
        public int Year { get; set; }
        public bool Exclude { get; set; }
        public bool OnlyRenewals { get; set; }
    }
    internal class JacException : JacCodeConfig
    {
        public string LocationName { get; set; }
        [IgnoreColumn]
        public string Period { get { return string.Format("{0} - {1}", Year - 1, Year); } }
    }
    [TableName("tjc_car_application_by_jac_code")]
    [PrimaryKey("JacCodeID,LocationID,ApplicationID", AutoIncrement = false)]
    internal class ApplicationJacCode
    {
        public int JacCodeID { get; set; }
        public int LocationID { get; set; }
        public int ApplicationID { get; set; }
        public int Status { get; set; }
    }
    internal class ApplicationJacCodeDetail
    {
        public int JacCodeID { get; set; }
        public int LocationID { get; set; }
        public int ApplicationID { get; set; }
        public int Status { get; set; }
        public string Category { get; set; }
        public int CaseTypeID { get; set; }
        public string CaseTypeName { get; set; }
        public string LocationName { get; set; }
    }
    public enum CodeStatus
    {
        New = 0,
        Approved = 1,
        Rejected = 2,
        Removed = 3,
        Locked = 4
    }
    public enum UpdateType
    {
        @new = 0,
        update = 1,
        remove = 2
    }
}
