using System.Xml;

namespace UpdateXMPFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessFile(@"C:\tt\ConsoleApplication1\UpdateXMPFiles\_DSC0001.xmp");
        }

        private static void ProcessFile(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("x", "adobe:ns:meta/");
            nsmgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

            var nodes = doc.DocumentElement.SelectNodes("/x:xmpmeta/rdf:RDF/rdf:Description", nsmgr);
            var exposure = nodes[0].Attributes["crs:Exposure2012"];
            exposure.Value = "100";

            var settings = new XmlWriterSettings();
            settings.NewLineOnAttributes = true;
            settings.Indent = true;

            var xmlWriter = XmlWriter.Create(@"C:\tt\ConsoleApplication1\UpdateXMPFiles\New_DSC0001.xmp", settings);
            doc.Save(xmlWriter);
            xmlWriter.Close();
        }
    }
}
