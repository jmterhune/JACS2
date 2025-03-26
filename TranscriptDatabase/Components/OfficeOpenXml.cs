using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Runtime.Remoting.Contexts;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class OfficeOpenXml
    {
        private string tempfile = HttpContext.Current.Server.MapPath("~/temp/tempWordDoc.docx");
        public Package CreateOpenPackage()
        {

            // Set the proper Name for the file based on the type
            string filename = string.Empty;

            filename = tempfile;

            // Return the opened package
            return Package.Open(filename, FileMode.Create, FileAccess.ReadWrite);
        }

        public Package CreateTemplatePackage(string filePath)
        {
            // Check to see if the file exsists and return the opened package
            if (File.Exists(filePath))
                return Package.Open(filePath, FileMode.Open, FileAccess.Read);

            // If the file does not exsist return nothing. 
            // NOTE: You Need code to handle this error. 
            return null/* TODO Change to default(_) if this is not a reference type */;
        }

        public  Package CopyTemplate(Package targetPackage, ref Package templatePackage)
        {
            // Select the type of document and set the values accordingly
            string tempContType = string.Empty;
            string newContType = string.Empty;
            tempContType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml";
            newContType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml";

            // Both packages should not be nothing so check them both and
            // then copy all package parts from the template to the target
            if (targetPackage != null && templatePackage != null)
            {
                Stream srcStream;
                Stream targetStream;

                // Copy package parts by looping through each part in the template package
                foreach (PackagePart p in templatePackage.GetParts())
                {

                    // Below statement allows us to save the target file as a regular 
                    // office(document) even if the template is a dotx or xltx file.
                    if (p.ContentType.ToString() == tempContType)
                    {
                        string updatedContentType = newContType;
                        targetStream = targetPackage.CreatePart(p.Uri, updatedContentType, CompressionOption.SuperFast).GetStream();
                    }
                    else
                        targetStream = targetPackage.CreatePart(p.Uri, p.ContentType, CompressionOption.SuperFast).GetStream();
                    srcStream = p.GetStream();

                    // Copy the template stream to the target stream
                    CopyStream(srcStream, targetStream);
                }

                // Close template package
                templatePackage.Close();
                try
                {
                    return targetPackage;
                }
                finally
                {
                }
            }
            else
            {
                // Close the template package
                templatePackage.Close();
                // Throw exception notifying user of invalid package
                throw new Exception("Invalid Package");
            }
        }

        private void CopyStream(Stream srcStream, Stream tgtStream)
        {
            byte[] buf = new byte[4096];
            int bytesRead = 0;
            srcStream.Position = 0;
            bytesRead = srcStream.Read(buf, 0, 0x1000);
            while ((bytesRead > 0))
            {
                tgtStream.Write(buf, 0, bytesRead);
                bytesRead = srcStream.Read(buf, 0, 0x1000);
            }
        }

        public void ClosePackage(ref Package package)
        {
            package.Flush();
            package.Close();
        }

        public void DisplayFile()
        {
            string filename = string.Empty;
            string contentType = string.Empty;
            string header = string.Empty;

            // Select the file type and set the needed variables
            filename = tempfile;
            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            header = "attachment;filename=ExportDocument.docx";
            System.IO.Stream myStream = File.Open(filename, FileMode.Open);
            byte[] b = new byte[System.Convert.ToInt32(myStream.Length) - 1 + 1];
            myStream.Read(b, 0, System.Convert.ToInt32(myStream.Length));
            myStream.Close();

            // Delete the temp workbook 
            System.IO.File.Delete(filename);

            // Stream the new workbook back to the client 
            HttpContext.Current.Response.ContentType = contentType;
            HttpContext.Current.Response.AddHeader("Content-Disposition", header);
            HttpContext.Current.Response.BinaryWrite(b);
            HttpContext.Current.Response.End();
        }

        public void ReplaceBookMark(ref Package targetPackage, string bookMarkName, string newValue)
        {

            // Check for valid package. Return exception if not valid
            if (targetPackage == null)
                throw new Exception("Invalid Package");

            // Get the main document xml document and load it into xDoc
            Uri docPartUri = PackUriHelper.CreatePartUri(new Uri("word/document.xml", UriKind.Relative));
            PackagePart docPart = targetPackage.GetPart(docPartUri);
            string docXml = new StreamReader(docPart.GetStream()).ReadToEnd();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(docXml);

            // Create the namespace manager needed for the OoXml xml prefixes 
            XmlNamespaceManager xmlNSMgr = new XmlNamespaceManager(xDoc.NameTable);
            xmlNSMgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            string nsUri = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            // Get the first style node for text runs. NOTE: if there are multiple styles in the document this will need to be changed
            XmlNode styleNode = xDoc.SelectSingleNode("//w:rPr[1]", xmlNSMgr);

            // Select the parent node of the bookmark start node
            XmlNode xNode = xDoc.SelectSingleNode("//w:bookmarkStart[@w:name='" + bookMarkName + "']/..", xmlNSMgr); // /w:r[w:t='" & bookMarkName & "']", xmlNSMgr)

            // Set the start and end bookmark nodes
            XmlNode startNode = xDoc.SelectSingleNode("//w:bookmarkStart[@w:name='" + bookMarkName + "']", xmlNSMgr);

            // Set the bookMark id 
            int bookmarkId = Int32.Parse(startNode.Attributes["w:id"].Value);

            // Get the ending bookmark node
            XmlNode endNode = xDoc.SelectSingleNode("//w:bookmarkEnd[@w:id='" + bookmarkId + "']", xmlNSMgr);

            // Loop through each node and grab the items beetween the start and end bookmark nodes
            List<XmlNode> nodearray = new List<XmlNode>();
            bool bmStart = false;
            bool bmEnd = false;
            foreach (XmlNode n in xNode)
            {
                if (n == startNode)
                    bmStart = true;
                if (n == endNode)
                    bmEnd = true;
                if (n != startNode && bmStart == true && bmEnd != true)
                    nodearray.Add(n);
            }

            // Replace anything within the bookmark with the new value
            if (nodearray.Count == 0)
            {
                // The below code executes if there is no text or nodes between
                // the start and stop nodes in the document

                XmlNode newNoder = xDoc.CreateElement("w", "r", nsUri);
                XmlNode newnodet = xDoc.CreateElement("w", "t", nsUri);
                newnodet.InnerText = newValue;
                // newNoder.AppendChild(newnodet)
                XmlNode commonParent = startNode.ParentNode;
                if (styleNode != null)
                {
                    XmlNode sNode = styleNode.Clone();
                    newNoder.AppendChild(sNode); // Inserts the style for the node
                    newNoder.AppendChild(newnodet); // Insert the text run node
                }
                else
                    newNoder.AppendChild(newnodet);// Insert the text run node
                commonParent.InsertAfter(newNoder, startNode);
            }
            else
                // Here the text is placed into the text run node
                for (int i = 0; i <= nodearray.Count - 1; i++)
                {
                    XmlNode n = nodearray[i];
                    if (n.Name == "w:r")
                    {
                        foreach (XmlNode cn in n)
                        {
                            if (cn.Name == "w:t")
                            {
                                cn.InnerText = newValue;
                                break; // Immediatly Exit the loop
                            }
                        }
                    }
                }

            // Save the Xml Document back to the package
            Stream updatedDocStream = docPart.GetStream(FileMode.Create, FileAccess.Write);
            xDoc.Save(updatedDocStream);
        }
    }
}