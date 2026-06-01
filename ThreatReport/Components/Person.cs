/*
' Copyright (c) 2019 jud12
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

namespace tjc.Modules.ThreatReport.Components
{
    [TableName("tjc_threat_person")]
    //setup the primary key for table
    [PrimaryKey("PersonID", AutoIncrement = true)]
    class Person
    {
        public int PersonID { get; set; }

        public int IncidentID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string Race { get; set; }

        public string Height { get; set; }

        public string Weight { get; set; }

        public string HairColor { get; set; }

        public string Features { get; set; }

        public string Phone { get; set; }

        public string Voice { get; set; }

        public string Vehicle { get; set; }


        public DateTime DateOfBirth { get; set; }
    }
}
