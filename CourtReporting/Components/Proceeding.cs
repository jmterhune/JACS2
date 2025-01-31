/*
' Copyright (c) 2020 12th Judicial Circuit Court
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
using System.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtReporting.Components
{
    [TableName("tjc_dcr_proceedings")]
    //setup the primary key for table
    [PrimaryKey("ProceedingID", AutoIncrement = true)]
    //configure caching using PetaPoco

   internal class Proceeding : ProceedingBase
    {
        public Proceeding()
        {
            Quantity = 1;
        }
    }
    public class ProceedingInfo : ProceedingBase
    {
    }
    public class ProceedingBase
    {
        public int ProceedingID { get; set; }

        public int RequestID { get; set; }

        public string ProceedingType { get; set; }

        public string ProceedingDate { get; set; }

        public string ProceedingTime { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [Required]
        public virtual int MediaTypeID
        {
            get
            {
                return (int)this.MediaType;
            }
            set
            {
                MediaType = (MediaTypes)value;
            }
        }
        [IgnoreColumn]
        [EnumDataType(typeof(MediaTypes))]
        public MediaTypes MediaType { get; set; }

        [IgnoreColumn]
        public string MediaTypeName
        {
            get
            {
                return Components.Helper.GetEnumDescription(MediaType);
            }
        }

        [IgnoreColumn]
        public decimal SubTotal
        {
            get
            {
                return (Price * Quantity);
            }
        }


    }
}
