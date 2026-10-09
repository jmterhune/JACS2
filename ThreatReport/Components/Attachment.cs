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
    [TableName("tjc_threat_attachment")]
    //setup the primary key for table
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    class Attachment
    {
        public int AttachmentID { get; set; }

        public int IncidentID { get; set; }

        public string Path { get; set; }

        public string URL { get; set; }

        public string FileName { get; set; }

        public DateTime UploadedDate { get; set; }

        public string ContentType { get; set; }

        // The file bytes live in the FileData column of the shared jud12 database. Ignored by the
        // repository so listing attachments does not load every blob; use
        // AttachmentController.GetFileData / SaveFileData instead.
        [IgnoreColumn]
        public byte[] FileData { get; set; }
    }
}
