/*
' Copyright (c) 2023 12th Judicial Circuit
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
namespace tjc.Intranet.API.Components.Mediation
{
    [TableName("tjc_med_attorneys")]
    [PrimaryKey("AttorneyId", AutoIncrement = true)]
    public class AttorneyListItem
    {
        public int AttorneyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Firm { get; set; }
        public string Phone { get; set; }
        public string Extension { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [IgnoreColumn]
        public string AttorneyName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return Firm;
                else if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    return string.Format("{0}, {1}", LastName, FirstName);
                else if (string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    return LastName;
                else if (!string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                    return FirstName;
                else
                    return string.Empty;
            }
        }
    }
}
