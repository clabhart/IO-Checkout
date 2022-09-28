using System;
using System.IO;
using System.Xml;

namespace IOCheckoutTool
{
    public class CP : IDisposable
    {
        #region Properties
        public XmlElement DA { get; set; }
        public XmlDocument DAFile { get; set; }
        public DirectoryInfo Project { get; set; }
        private string CPName { get; set; }
        #endregion Properties

        #region Public Constructors

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

        public CP()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DAFile.AppendChild(DA);
                DAFile.Save(Path.Combine(Project.FullName, string.Concat("Direct Access ", CPName, ".xml")));
            }
        }

        #endregion Protected Methods
    }
}