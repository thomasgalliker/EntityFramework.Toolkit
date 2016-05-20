using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.Extensions
{
    public static class EdmxTools
    {
        public static void UpdateEdmx(DbContext context, string edmxFilePath)
        {
            if (File.Exists(edmxFilePath))
            {
                var originalDoc = XDocument.Load(edmxFilePath);
                var originalDesignerElement = originalDoc.Root.Elements().Single(e => e.Name.LocalName == "Designer");

                var newDoc = GetModel(context);
                var newDesignerElement = newDoc.Root.Elements().Single(e => e.Name.LocalName == "Designer");

                newDesignerElement.ReplaceWith(originalDesignerElement);

                newDoc.Save(edmxFilePath);
            }
            else
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter writer = XmlWriter.Create(edmxFilePath, settings))
                {
                    EdmxWriter.WriteEdmx(context, writer);
                }
            }
        }

        public static XDocument GetModel(DbContext context)
        {
            return GetModel(w => EdmxWriter.WriteEdmx(context, w));
        }

        public static XDocument GetModel(Action<XmlWriter> writeXml)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(
                    memoryStream, new XmlWriterSettings
                    {
                        Indent = true
                    }))
                {
                    writeXml(xmlWriter);
                }

                memoryStream.Position = 0;

                return XDocument.Load(memoryStream);
            }
        }

        public static void CreateDatabaseScript(DbContext context, string sqlFilePath)
        {
            string script = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();

            using (var writer = new StreamWriter(sqlFilePath))
            {
                writer.Write(script);
            }
        }
    }
}
