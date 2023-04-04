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
    [TableName("jacs.TBCaseCycle")]
    
    //setup the primary key for table
    [PrimaryKey("CaseCycle_Id", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class CaseCycle
    {
        public int CaseCycle_Id { get; set; }
        public string CaseNum { get; set; }
        public int FLRC_Id { get; set; }
        public DateTime lupddate { get; set; }

    }
}
