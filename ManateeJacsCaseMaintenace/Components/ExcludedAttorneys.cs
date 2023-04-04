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
    [TableName("dbo.tjc_excluded_barnumbers")]
    //configure caching using PetaPoco
    [PrimaryKey("RecordId", AutoIncrement = true)]
    internal class ExcludedAttorney
    {
        public int RecordId { get; set; }
        public string barnumber { get; set; }
    }

    [TableName("jacs.tjc_excluded_attorneys")]
    //configure caching using PetaPoco
    [PrimaryKey("RecordId", AutoIncrement = false)]
    internal class ExcludedAttorneyView
    {
        public int RecordId { get; set; }
        public string barnumber { get; set; }
        public string ACTIVE { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
    }
}
