using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace TriResultsCsvReader
{
    public class ColumnsConfigReader
    {
        public IEnumerable<Column> ReadFile(string configFilename)
        {
            return Read(File.ReadAllText(configFilename));
        }

        public IEnumerable<Column> Read(string xmlConfig)
        {
            var doc = XDocument.Parse(xmlConfig);

            var columns =
                from column in doc.Element("columns").Elements("column")
                let name = column.Element("name")?.Value
                let altNames = column.Element("mapfrom")?.Elements() ?? new List<XElement>()
                let names = altNames.Select(elem => elem.Value)
                select new Column() {Name = name, AlternativeNames = names};

            return columns;
        }
    }
}
