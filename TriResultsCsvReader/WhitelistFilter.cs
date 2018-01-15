using System;
using System.Collections.Generic;
using System.Linq;

namespace TriResultsCsvReader
{
    public class WhitelistFilter : IWhitelistFilter
    {
        private readonly Dictionary<string, string> _allowedNames;

        public WhitelistFilter(IEnumerable<string> allowedNames)
        {
            try {
                _allowedNames = allowedNames.ToDictionary(n => n);
            } catch(ArgumentException ex)
            {
                var dupes = allowedNames.GroupBy(x => x).Select(x => new { Name = x.First(), Count = x.Count() })
                    .Where(x => x.Count > 1);
                Console.WriteLine("Duplicates found: {0}", string.Join(", ", dupes.Select(x => x.Name)));
                throw;
            }
        }

        public bool ExactMatch(string name)
        {
            return name != null && _allowedNames.ContainsKey(name);
        }

        public bool StartsWithMatch(string name)
        {
            return _allowedNames.Keys.Any(a => name != null && name.StartsWith(a));
        }

        public bool ContainsMatch(string name)
        {
            return _allowedNames.Keys.Any(key => name != null && name.Contains(key));
        }

        public IEnumerable<string> GetEntries()
        {
            return _allowedNames.Keys;
        }
    }
}
