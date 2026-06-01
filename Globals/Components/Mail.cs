using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;

namespace tjc.Modules.Globals.Components
{
    public class SendBulkMail
    {
        public string FromAddress { get; set; }
        private readonly List<string> _Recipients = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SendResult { get; set; }
        public MailAttachment Attachment { get; set; }
        public string SendEmails()
        {
            string result = "OK";
            try
            {
                foreach (string toAddress in _Recipients)
                {
                    string subject = Subject;
                    string body = Body;
                    string fromAddress = FromAddress;
                    if (Attachment != null && Attachment.Filename != "")
                    {
                        List<MailAttachment> attachments = new List<MailAttachment>();
                        {
                            attachments.Add(Attachment);
                        }

                        Mail.SendEmail(fromAddress, FromAddress, toAddress, subject, body, attachments);
                    }
                    else
                    {
                        Mail.SendEmail(fromAddress, toAddress, subject, body);
                    }
                }
            }
            catch (Exception exc)
            {

                result = exc.Message;
            }
            return result;
        }
        public void AddEmailAddress(string email)
        {
            _Recipients.Add(email);
        }
        public void Send()
        {
            SendResult = SendEmails();
        }
    }
}