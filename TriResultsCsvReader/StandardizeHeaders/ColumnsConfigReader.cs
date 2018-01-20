using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace TriResultsCsvReader
{
    public class ColumnsConfigReader
    {
        public IEnumerable<Column> ReadFile(string configFilename)
        {

            if (string.IsNullOrEmpty(configFilename))
            {
                throw new BadConfigurationException("No config file given");
            }

            if (!File.Exists(configFilename))
            {
                var errorMessage = $"Config file not found in path: {configFilename}";
                throw new BadConfigurationException(errorMessage);
            }

            return Read(File.ReadAllText(configFilename));
        }

        public IEnumerable<Column> Read(string xmlConfig)
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xmlConfig);
            }
            catch (XmlException ex)
            {
                Console.WriteLine("Invalid XML: {0}, {1}", ex.Message, xmlConfig);
            }

            var columns = from column in doc.Element("columns").Elements("column")
                          let name = column.Element("name")?.Value
                          let altNames = column.Element("mapfrom")?.Elements() ?? new List<XElement>()
                          let names = altNames.Select(elem => elem.Value)
                          select new Column() { Name = name, AlternativeNames = names };
            
            return columns;
        }
    }
}
