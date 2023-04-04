/*
' Copyright (c) 2023 jterhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.JacsCaseMaint.Components
{
    [TableName("jacs.TBCases")]
    //configure caching using PetaPoco
    [PrimaryKey("CASENUM", AutoIncrement = false)]
    internal class Cases
    {
        public string CASENUM { get; set; }
        public string BARNUM { get; set; }
        public int CASEID { get; set; }
        public string MOTIONCODE { get; set; }
        public string PLAINTIFF { get; set; }
        public string OPPOSINGBARNUM { get; set; }
        public string DEFENDANT { get; set; }
        public string ATTORNEYNAME { get; set; }
        public string OPPATTORNEYNAME { get; set; }
        public DateTime LUPDDATE { get; set; }
    }
}
