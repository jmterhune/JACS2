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
using System.Collections.Generic;

namespace tjc.Modules.ExitSurvey.Components
{
    // A single row of a satisfaction matrix or a checkbox/radio option.
    public class SurveyItem
    {
        public string Key { get; set; }
        public string Label { get; set; }

        public SurveyItem(string key, string label)
        {
            Key = key;
            Label = label;
        }
    }

    // The fixed structure of the Twelfth Judicial Circuit Employee Exit Survey.
    // Kept in one place so the entry form and the admin detail view stay in sync.
    public static class SurveyDefinition
    {
        public const string SectionGeneral = "general";
        public const string SectionSupervision = "supervision";

        // The 5-point scale shared by Q1 and Q6 (value 1..5).
        public static readonly List<SurveyItem> RatingScale = new List<SurveyItem>
        {
            new SurveyItem("1", "Very Satisfied"),
            new SurveyItem("2", "Satisfied"),
            new SurveyItem("3", "Neutral"),
            new SurveyItem("4", "Dissatisfied"),
            new SurveyItem("5", "Very Dissatisfied"),
        };

        // Q1 - general conditions of the job.
        public static readonly List<SurveyItem> GeneralConditions = new List<SurveyItem>
        {
            new SurveyItem("physical_conditions", "Physical working conditions"),
            new SurveyItem("type_of_work", "Type of work (job content, duties, etc.)"),
            new SurveyItem("volume_of_work", "Volume of work"),
            new SurveyItem("job_security", "Job security"),
            new SurveyItem("policies_procedures", "Court/office policies and procedures"),
            new SurveyItem("coworkers", "Relationship with co-workers"),
            new SurveyItem("supervisor", "Relationship with supervisor"),
            new SurveyItem("challenge", "Challenge of work"),
            new SurveyItem("importance", "Importance of work"),
            new SurveyItem("responsibilities", "Job responsibilities"),
            new SurveyItem("training_development", "Training/personal development"),
            new SurveyItem("accomplishments", "Accomplishments of work unit"),
            new SurveyItem("pay", "Pay received for work performed"),
            new SurveyItem("advancement", "Advancement/promotional opportunities"),
            new SurveyItem("other_benefits", "Other benefits (retirement, insurance, leave, etc.)"),
            new SurveyItem("overall", "Overall working for the Court"),
            new SurveyItem("other", "Other"),
        };

        // Q6 - supervision received.
        public static readonly List<SurveyItem> SupervisionItems = new List<SurveyItem>
        {
            new SurveyItem("goals_defined", "Goals, expectations were clearly defined"),
            new SurveyItem("utilization", "Utilization of your abilities"),
            new SurveyItem("assistance_amount", "Amount of assistance received"),
            new SurveyItem("assistance_effectiveness", "Effectiveness of assistance received"),
            new SurveyItem("progress_interest", "Interest taken in your progress"),
            new SurveyItem("fair_treatment", "Fair and impartial treatment"),
            new SurveyItem("recognition", "Recognition, appreciation of your ideas, accomplishments"),
        };

        // Q4 - type of employer of the accepted position.
        public static readonly List<SurveyItem> EmployerTypes = new List<SurveyItem>
        {
            new SurveyItem("state_government", "State Government"),
            new SurveyItem("county_government", "County Government"),
            new SurveyItem("city_government", "City Government"),
            new SurveyItem("private_sector", "Private Sector"),
            new SurveyItem("non_profit", "Non-Profit Organization"),
            new SurveyItem("other_court", "Another Court or office within State Courts System"),
            new SurveyItem("other", "Other"),
        };

        // Q5 - what influenced you to leave (multi-select).
        public static readonly List<SurveyItem> LeaveReasons = new List<SurveyItem>
        {
            new SurveyItem("better_conditions", "Better working conditions"),
            new SurveyItem("more_challenging", "More challenging position"),
            new SurveyItem("greater_security", "Greater job security"),
            new SurveyItem("advancement", "Better chance for advancement"),
            new SurveyItem("higher_salary", "Higher salary"),
            new SurveyItem("better_coworkers", "Better relationship with other employees"),
            new SurveyItem("better_supervisor", "Better relationship with supervisor"),
            new SurveyItem("more_work", "More work"),
            new SurveyItem("less_work", "Less work"),
            new SurveyItem("more_hours", "More hours"),
            new SurveyItem("fewer_hours", "Fewer hours"),
            new SurveyItem("other", "Other"),
        };
    }
}
