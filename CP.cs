using System;
using System.IO;
using System.Xml;

namespace IOCheckoutTool
{
    public class CP : IDisposable
    {
        public XmlDocument DAFile { get; set; }
        public XmlElement DA { get; set; }
        public DirectoryInfo Project { get; set; }
        private string CPName { get; set; }

        public CP(string cp)
        {
            CPName = cp;
            DAFile = new XmlDocument()
            {
                XmlResolver = null
            };
            _ = DAFile.DocumentElement;
            DA = DAFile.CreateElement("DirectAccess");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DAFile.AppendChild(DA);
                DAFile.Save(Path.Combine(Project.FullName, string.Concat("Direct Access ", CPName, ".xml")));
            }
        }
    }
}