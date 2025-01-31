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
using System.Web.Caching;

namespace tjc.Modules.ProSeLog.Components
{
    [TableName("tjc_prose_case_type")]
    //setup the primary key for table
    [PrimaryKey("CaseTypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("CaseTypes", CacheItemPriority.Default, 20)]
    internal class CaseType
    {
        public int CaseTypeID { get; set; }

        public string CaseTypeName { get; set; }

    }
    [TableName("tjc_prose_case_numbers")]
    public class CaseNumber
    {
        public string Text { get; set; }
    }
}
