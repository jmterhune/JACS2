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
    [TableName("tjc_car_settings")]
    //setup the primary key for table
    [PrimaryKey("SettingID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class Setting
    {
        public int SettingID { get; set; }
        public string VerificationNote { get; set; }
        public string EditAttorneyNote { get; set; }
        public string EditApplicationNote { get; set; }
        public string ApplicationEmail { get; set; }
        public string UpdateNotificationSendTo { get; set; }
        public string ContactEmail { get; set; }
        public int BeginFiscalYearMonth { get; set; }
        public int BeginFiscalYearDay { get; set; }
    }
}
