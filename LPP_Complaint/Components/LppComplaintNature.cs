/*
' Copyright (c) 2026  12th Judicial Circuit
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

namespace tjc.Modules.LPP_Complaint.Components
{
    /// <summary>
    /// One ticked box from the complaint form's "Nature of Incident" list. The form
    /// allows several, so they live in their own table rather than a delimited column,
    /// which keeps them countable for reporting.
    /// </summary>
    [TableName("tjc_modules_lpp_complaint_natures")]
    [PrimaryKey("LppComplaintNatureId", AutoIncrement = true)]
    public class LppComplaintNature
    {
        public int LppComplaintNatureId { get; set; }

        public int LppComplaintId { get; set; }

        public string Nature { get; set; }
    }
}
