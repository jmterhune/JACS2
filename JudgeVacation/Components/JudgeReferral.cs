/*
' Copyright (c) 2022 Joe Terhune
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

namespace tjc.Modules.JudgeVacation.Components
{
    [TableName("tjc_judicial_referral")]
    //setup the primary key for table
    [PrimaryKey("ReferralID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Referrals", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class JudgeReferral
    {       
        public int ReferralID { get; set; }

        #region JA Fields
        public int JaID { get; set; }
        public int JudgeID { get; set; }
        public string CaseType { get; set; }
        public string CaseParties { get; set; }
        public string CaseNumber { get; set; }
        public string MotionTitle { get; set; }
        public DateTime? MotionDate { get; set; }
        public bool MotionVacate { get; set; }
        public bool MotionCorrect { get; set; }
        public bool MotionDirected { get; set; }
        public string DirectedMotions { get; set; }
        public bool MotionOther { get; set; }
        public DateTime JaCreatedDate { get; set; }

        [IgnoreColumn]
        public string JudgeName
        {
            get
            {
                var judgeUser = DotNetNuke.Entities.Users.UserController.Instance.GetUserById(0, JudgeID);
                if (judgeUser != null)
                    return judgeUser.DisplayName;
                return "";
            }
        }
        #endregion

        #region Judge Response

        public bool CounselAssistance { get; set; }
        public string JudgeMotions { get; set; }
        public DateTime? JudgeResponseDate { get; set; }
        public Statuses Status { get; set; }

        [IgnoreColumn]
        public string StatusName
        {
            get
            {
                switch (Status)
                {
                    case Statuses.NewReferral:
                        return "New";
                    case Statuses.ReferredToCounsel:
                        return "Referred to Court Counsel";
                    case Statuses.RetainedByJudge:
                        return "Retained by Judge";
                    case Statuses.Complete:
                        return "Complete";
                    default:
                        return "";
                }
            }
        }

        #endregion

        #region Counsel Response
        public DateTime? CounselReceivedDate { get; set; }
        #endregion
        public enum Statuses
        {
            NewReferral = 1,
            ReferredToCounsel = 2,
            RetainedByJudge = 3,
            Complete = 4
        }
    }
}
