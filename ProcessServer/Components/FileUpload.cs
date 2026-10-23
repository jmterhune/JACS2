/*
' Copyright (c) 2019 12th Judicial Circuit Court
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

namespace tjc.Modules.ProcessServer.Components
{
    [TableName("tjc_process_server_attachments")]
    [PrimaryKey("AttachmentID", AutoIncrement = true)]
    class UploadedFile
    {
        public int AttachmentID { get; set; }
        public int ProcessServerID { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool Hidden { get; set; }
        public string FullPath { get; set; }
        public byte[] FileData { get; set; }
        [IgnoreColumn]
        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(FullPath);
            }
        }
        [IgnoreColumn]
        public string Extension
        {
            get
            {
                return System.IO.Path.GetExtension(FullPath);
            }
        }
        [IgnoreColumn]
        public string OriginalFileName
        {
            get
            {
                int index = FileName.IndexOf('_', FileName.IndexOf('_') + 1);
                return FileName.Substring(index + 1);
            }
        }

    }
}
