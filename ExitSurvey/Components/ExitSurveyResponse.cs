/*
' Copyright (c) 2026  Joe Terhune
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

namespace tjc.Modules.ExitSurvey.Components
{
    // One row per completed exit survey. Scalar answers live here; the two matrix
    // questions (Q1 general conditions, Q6 supervision) and the Q5 reason checkboxes
    // are normalized into the rating/reason child tables.
    [TableName("tjc_exit_survey_response")]
    [PrimaryKey("ResponseID", AutoIncrement = true)]
    public class ExitSurveyResponse
    {
        public int ResponseID { get; set; }
        public int ModuleID { get; set; }

        // Page 1 opt-out checkbox
        public bool DoNotShareWithSupervisor { get; set; }

        // Q1 "Other (specify)" free-text label (the rating itself is a rating row)
        public string Q1OtherSpecify { get; set; }

        // Q2 advantages of the new employer are multi-select (stored as "advantages"
        // rows in tjc_exit_survey_reason); this holds only the "Other (specify)" text.
        public string Q2AdvantagesOther { get; set; }

        // Q3 have you accepted another position?
        public bool? Q3AcceptedPosition { get; set; }

        // Q4 type of employer accepted (single choice) + "Other (specify)"
        public string Q4EmployerType { get; set; }
        public string Q4EmployerTypeOther { get; set; }

        // Q5 "Other (specify)" free text (selected reasons are reason rows)
        public string Q5ReasonsOther { get; set; }

        // Q7 adequate training / resources
        public string Q7Training { get; set; }

        // Q8 what could make the Circuit a better place to work
        public string Q8Comments { get; set; }

        // Q9 would you consider working here again?
        public bool? Q9WouldReturn { get; set; }

        // Optional identifying section
        public string OptName { get; set; }
        public string OptPositionTitle { get; set; }
        public string OptSupervisorName { get; set; }
        public string OptSupervisorTitle { get; set; }
        public string OptHiredMonthYear { get; set; }
        public string OptSeparatedMonthYear { get; set; }
        public string OptDateCompleted { get; set; }

        // Audit
        public int? CreatedByUserId { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }

    // A single cell of a satisfaction matrix (Q1 or Q6).
    // Rating: 1 = Very Satisfied ... 5 = Very Dissatisfied, 0 = not answered.
    [TableName("tjc_exit_survey_rating")]
    [PrimaryKey("RatingID", AutoIncrement = true)]
    public class ExitSurveyRating
    {
        public int RatingID { get; set; }
        public int ResponseID { get; set; }
        public string Section { get; set; }
        public string ItemKey { get; set; }
        public string ItemLabel { get; set; }
        public int Rating { get; set; }

        [IgnoreColumn]
        public string RatingDisplay
        {
            get
            {
                switch (Rating)
                {
                    case 1: return "Very Satisfied";
                    case 2: return "Satisfied";
                    case 3: return "Neutral";
                    case 4: return "Dissatisfied";
                    case 5: return "Very Dissatisfied";
                    default: return "";
                }
            }
        }
    }

    // A checked option from a multi-select question. Section distinguishes the
    // Q2 advantages ("advantages") from the Q5 leave reasons ("leave").
    [TableName("tjc_exit_survey_reason")]
    [PrimaryKey("ReasonID", AutoIncrement = true)]
    public class ExitSurveyReason
    {
        public const string SectionAdvantages = "advantages";
        public const string SectionLeave = "leave";

        public int ReasonID { get; set; }
        public int ResponseID { get; set; }
        public string Section { get; set; }
        public string ReasonKey { get; set; }
        public string ReasonLabel { get; set; }
    }
}
