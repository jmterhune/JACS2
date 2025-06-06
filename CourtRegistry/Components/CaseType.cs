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
    [TableName("tjc_car_case_types")]
    //setup the primary key for table
    [PrimaryKey("CaseTypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class CaseType
    {
        public int CaseTypeID { get; set; }
        public string CaseTypeName { get; set; }
        public string LabelNote { get; set; }
        public bool Active { get; set; }
       
    }
}
