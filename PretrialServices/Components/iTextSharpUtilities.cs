using System;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using System.Management.Instrumentation;
using DotNetNuke.Services.Exceptions;

namespace tjc.Modules.PretrialServices.Components
{
    public class TwoColumnHeaderFooter : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        private PdfContentByte contentByte;

        // we will put the final number of pages in a template
        private PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        private BaseFont baseFont = null;

        // This keeps track of the creation time
        private DateTime printTime = DateTime.Now;

        public string Title
        {
            get;set;
        }

        public string HeaderLeft
        {
            get; set;
        }

        public string HeaderRight
        {
            get; set;
        }

        public Font HeaderFont
        {
            get; set;
        }

        public Font FooterFont
        {
            get; set;
        }

        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                printTime = DateTime.Now;
                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                contentByte = writer.DirectContent;
                template = contentByte.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
                Exceptions.LogException(de);
            }
            catch (IOException ioe)
            {
                Exceptions.LogException(ioe);
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            Rectangle pageSize = document.PageSize;

            if (Title != string.Empty)
            {
                contentByte.BeginText();
                contentByte.SetFontAndSize(baseFont, 10);
                contentByte.SetTextMatrix(pageSize.GetLeft(10), pageSize.GetTop(30));
                contentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER, Title, pageSize.GetLeft(400), pageSize.GetTop(30), 0);
                contentByte.EndText();
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = baseFont.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;
            contentByte.SetRGBColorFill(100, 100, 100);
            contentByte.BeginText();
            contentByte.SetFontAndSize(baseFont, 8);
            contentByte.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
            contentByte.ShowText(text);
            contentByte.EndText();
            contentByte.AddTemplate(template, pageSize.GetLeft(40) + len, pageSize.GetBottom(30));
            contentByte.BeginText();
            contentByte.SetFontAndSize(baseFont, 8);
            contentByte.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Printed On " + printTime.ToString(), pageSize.GetRight(40), pageSize.GetBottom(30), 0);
            contentByte.EndText();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(baseFont, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + Convert.ToString((writer.PageNumber)));
            template.EndText();
        }
    }
}
