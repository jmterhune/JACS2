// File: Components\NameMatchResult.cs
/*
' Copyright (c) 2026 Joe Terhune
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

namespace tjc.Modules.DocketInmateCompare.Components
{
    [TableName("tjc_inmate_matches")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Cacheable("InmateMatches", CacheItemPriority.Default, 20)]
    public class NameMatchResult
    {
        public int Id { get; set; }

        public Guid SetGuid { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public string CourtName { get; set; }

        public string CourtCase { get; set; }

        public string JailName { get; set; }

        public string JailCase { get; set; }

        public double Similarity { get; set; }

        public string Start { get; set; }

        public string MotionType { get; set; }

        public string EventType { get; set; }

        public string Mode { get; set; } = "Zoom";

        public NameMatchResult()
        {
        }

        public NameMatchResult(string courtName, string jailName, double similarity)
        {
            CourtName = courtName;
            JailName = jailName;
            Similarity = similarity;
        }

        public override string ToString()
        {
            return $"Court: {CourtName} | Jail: {JailName} | Match: {Similarity:P0}";
        }
    }

    public class CourtEntry
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string OriginalCase { get; set; }
        public string CaseNum { get; set; }
        public string Start { get; set; }
        public string MotionType { get; set; }
        public string EventType { get; set; }
    }

    public class JailEntry
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string OriginalCase { get; set; }
        public string CaseNum { get; set; }
    }
}