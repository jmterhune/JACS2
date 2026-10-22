using System.Collections.Generic;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.API.SWNResponse from
    /// D:\websites\Intranet\App_Code\EmployeeDB\SWNServiceRequests.vb.
    /// Wrapper around a list of result messages with a convenience error flag.
    /// </summary>
    public class SWNResponse
    {
        public SWNResponse()
        {
            MessageList = new List<SWNResponseMessage>();
        }

        public SWNResponse(bool hasErrors, List<SWNResponseMessage> messageList)
        {
            HasErrors = hasErrors;
            MessageList = messageList ?? new List<SWNResponseMessage>();
        }

        public bool HasErrors { get; set; }
        public List<SWNResponseMessage> MessageList { get; set; }
    }
}
