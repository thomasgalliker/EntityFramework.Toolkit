using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace EntityFramework.Toolkit
{
    public static class EdmxTools
    {
        public static void UpdateEdmx(DbContext context, string edmxFilePath)
        {
            if (File.Exists(edmxFilePath))
            {
                XNamespace xmlRootNamespace = "http://schemas.microsoft.com/ado/2009/11/edmx";
                var originalDoc = XDocument.Load(edmxFilePath);
                var originalDesignerElement = originalDoc.Root.Elements().Single(e => e.Name.LocalName == "Designer");

                var newDoc = GetModel(context);
                var newDesignerElement = newDoc.Root.Elements().Single(e => e.Name.LocalName == "Designer");
                var newRuntimeElement = newDoc.Root.Elements().Single(e => e.Name.LocalName == "Runtime");
                var newSchema = newRuntimeElement.Elements().Single(e => e.Name.LocalName == "ConceptualModels")
                    .Elements().Single(e => e.Name.LocalName == "Schema");

                var entityNamespace = newSchema.Attributes().Single(a => a.Name.LocalName == "Namespace").Value;

                var diagram = originalDesignerElement
                    .Elements().Single(e => e.Name.LocalName == "Diagrams")
                    .Elements().Single(e => e.Name.LocalName == "Diagram");

                double width = 1.5d;
                double startPointX = 0.0d;
                double startPointY = 0.0d;

                var newEntityTypes = newSchema.Elements().Where(e => e.Name.LocalName == "EntityType").ToList();
                var existingEntityShapes = diagram.Elements().Where(e => e.Name.LocalName == "EntityTypeShape").ToList();
                foreach (var newEntityType in newEntityTypes)
                {
                    var name = newEntityType.Attribute(XName.Get("Name")).Value;
                    if (!existingEntityShapes.Any(e => e.Attribute(XName.Get("EntityType")).Value.Contains(name)))
                    {
                        // Add new EntityTypeShape
                        diagram.Add(
                            new XElement(xmlRootNamespace + "EntityTypeShape",
                                new XAttribute("EntityType", entityNamespace + "." + name),
                                new XAttribute("Width", width.ToString(CultureInfo.InvariantCulture)),
                                new XAttribute("PointX", startPointX.ToString(CultureInfo.InvariantCulture)),
                                new XAttribute("PointY", startPointY.ToString(CultureInfo.InvariantCulture)),
                                new XAttribute("IsExpanded", "true")));

                        startPointX += width;
                    }
                }

                // Remove outdated EntityTypeShapes
                foreach (var existingEntityShape in existingEntityShapes)
                {
                    var name = existingEntityShape.Attribute(XName.Get("EntityType")).Value;
                    if (!newEntityTypes.Any(e => name.Contains(e.Attribute(XName.Get("Name")).Value)))
                    {
                        diagram.Elements().Where(e => e == existingEntityShape).Remove();
                    }
                }

                var newAssociations = newSchema.Elements().Where(e => e.Name.LocalName == "Association").ToList();
                var existingAssociations = diagram.Elements().Where(e => e.Name.LocalName == "AssociationConnector").ToList();
                foreach (var newAssociation in newAssociations)
                {
                    var name = newAssociation.Attribute(XName.Get("Name")).Value;
                    if (!existingAssociations.Any(e => e.Attribute(XName.Get("Association")).Value.Contains(name)))
                    {
                        // Add new AssociationConnector
                        diagram.Add(
                            new XElement(xmlRootNamespace + "AssociationConnector",
                                new XAttribute("Association", entityNamespace + "." + name),
                                new XAttribute("ManuallyRouted", "false")));
                    }
                }

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
