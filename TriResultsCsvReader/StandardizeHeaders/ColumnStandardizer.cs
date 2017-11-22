using System;
using System.Collections.Generic;
using System.Linq;

namespace TriResultsCsvReader
{

    public interface INameMapper
    {
        IEnumerable<string> GetStandardColumnNames(IEnumerable<string> columnNames);
    }

    /// <summary>
    /// Replaced mapped columns with standardized column names.
    /// Unknown columns are kept to keep the number of columns the same
    /// </summary>
    public class ColumnStandardizer : INameMapper
    {
        private readonly IEnumerable<Column> _columnConfig;

        public ColumnStandardizer(IEnumerable<Column> columnConfig)
        {
            _columnConfig = columnConfig;
        }

        public IEnumerable<string> GetStandardColumnNames(IEnumerable<string> columnNames)
        {
            return GetStandardColumnNames(columnNames, _columnConfig);
        }

        public IEnumerable<string> GetStandardColumnNames(IEnumerable<string> columnNames, IEnumerable<Column> columnConfig)
        {
            var results = new List<string>();

            var mapping = InvertColumnConfig(columnConfig.ToList());

            foreach (var columnName in columnNames)
            {
                if (mapping.TryGetValue(columnName, out var standardizedName))
                {
                    results.Add(standardizedName);
                }
                else
                {
                    results.Add(columnName);
                }
            }
            return results;
        }

        private Dictionary<string, string> InvertColumnConfig(IEnumerable<Column> columnConfig)
        {
            var mapping = new Dictionary<string,string>();

            // keys are all columns that can be mapped, including the standardized names
            foreach (var col in columnConfig)
            {
                foreach (var alternativeName in col.AlternativeNames)
                {
                    if (mapping.ContainsKey(alternativeName))
                    {
                        throw new BadConfigurationException(
                            $"Duplicate column name in configuration: {alternativeName}");
                    }
                    mapping.Add(alternativeName, col.Name);
                }

                mapping.Add(col.Name, col.Name);
            }

            return mapping;
        }
    }
}
