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
            _allowedNames = allowedNames.ToDictionary(n => n);
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
