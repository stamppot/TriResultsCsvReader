using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class DateUtils
    {
        public static string ReplaceStringMonth(string name)
        {
            return name
                .Replace("-jan-", "-01-")
                .Replace("-feb-", "-02-")
                .Replace("-mar-", "-03-")
                .Replace("-apr-", "-04-")
                .Replace("-may-", "-05-")
                .Replace("-jun-", "-06-")
                .Replace("-jul-", "-07-")
                .Replace("-aug-", "-08-")
                .Replace("-sep-", "-09-")
                .Replace("-oct-", "-10-")
                .Replace("-nov-", "-11-")
                .Replace("-dec-", "-12-");
        }
    }
}
