namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.API.SWNResponsMessageType (intentional VB typo preserved
    /// as SWNResponseMessageType on the C# side).
    /// </summary>
    public enum SWNResponseMessageType
    {
        Failure = 0,
        Information = 1,
        Warning = 2
    }

    /// <summary>
    /// Port of AWS.SWN.API.SWNResponseMessge (sic) from
    /// D:\websites\Intranet\App_Code\EmployeeDB\SWNServiceRequests.vb.
    /// The VB class had a typo (SWNResponseMessge); we use SWNResponseMessage.
    /// </summary>
    public class SWNResponseMessage
    {
        public SWNResponseMessage()
        {
        }

        public SWNResponseMessage(SWNResponseMessageType messageType, string messageText)
        {
            MessageType = messageType;
            MessageText = messageText;
        }

        public SWNResponseMessageType MessageType { get; set; }
        public string MessageText { get; set; }
    }
}
