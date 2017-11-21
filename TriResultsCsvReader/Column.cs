using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class Column
    {
        public string Name { get; set; }

        public IEnumerable<string> AlternativeNames { get; set; }

        public string GetStandardizedName(string alternativeName)
        {
            if (Name == alternativeName)
                return Name;

            if (AlternativeNames.Any(x => x == alternativeName))
            {
                return Name;
            }

            return string.Empty;
        }
    }
}
