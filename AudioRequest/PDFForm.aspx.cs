namespace tjc.Modules.AudioRequest
{
    public partial class PDFForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string jurisdiction = Request.QueryString["jurisdiction"];
            string procType = Request.QueryString["proctype"];
            string cdtype = Request.QueryString["cdtype"];
            string reqname = Request.QueryString["reqname"];
            string reqfirm = Request.QueryString["reqfirm"];
            string reqAddress = Request.QueryString["reqaddress"];
            string reqcity = Request.QueryString["reqcity"];
            string reqphone = Request.QueryString["reqphone"];

            string reqEmail = Request.QueryString["reqemail"];
            string caseName = Request.QueryString["casename"];
            string caseNumber = Request.QueryString["casenumber"];
            string judge = Request.QueryString["judge"];
            string procDate = Request.QueryString["procdate"];
            string procTime = Request.QueryString["proctime"];
            string procLocation = Request.QueryString["proclocation"];
            string involvement = Request.QueryString["involvement"];
            string delivery = Request.QueryString["delivery"];
            string transcrip = Request.QueryString["transcrip"];

            if (!Page.IsPostBack)
            {
                string template = "";
                if (procType.Contains("Juvenile") | procType.Contains("Adoption") | procType.Contains("TPR"))
                {
                    template = Server.MapPath("~/forms/juvenileaudio.pdf");

                }
                else
                {
                    template = Server.MapPath("~/forms/audio.pdf");
                }

                // create a new PDF reader based on the PDF template document

                PdfReader pdfReader = new PdfReader(template);
                MemoryStream output = new MemoryStream();
                PdfStamper stamper = new PdfStamper(pdfReader, output);
                AcroFields pdfFormFields = stamper.AcroFields;
                pdfFormFields.SetField("reqphone", reqphone);
                pdfFormFields.SetField("jurisdiction", jurisdiction);
                pdfFormFields.SetField("reqname", reqname);
                pdfFormFields.SetField("casename", caseName);
                pdfFormFields.SetField("delivery", delivery);
                pdfFormFields.SetField("date", DateTime.Today.ToShortDateString());
                pdfFormFields.SetField("transcriptionist", transcrip);
                pdfFormFields.SetField("reqemail", reqEmail);
                pdfFormFields.SetField("cdtype", cdtype);
                pdfFormFields.SetField("proctime", procTime);
                pdfFormFields.SetField("reqfirm", reqfirm);
                pdfFormFields.SetField("proclocation", procLocation);
                pdfFormFields.SetField("involvement", involvement);
                pdfFormFields.SetField("proctype", procType);
                pdfFormFields.SetField("procdate", procDate);
                pdfFormFields.SetField("judge", judge);
                pdfFormFields.SetField("casenumber", caseNumber);
                pdfFormFields.SetField("reqcity", reqcity);
                pdfFormFields.SetField("reqaddress", reqAddress);
                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();
                Response.AddHeader("Content-Disposition", "inline; filename=AudioRequest.pdf");
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(output.ToArray());
                Response.End();
            }
        }
    }
}