using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public static class QueryExtensions
    {
        public static IEnumerable<ResultRow> WhereWhitelistedName(
            this IEnumerable<ResultRow> query, IWhitelistFilter nameWhitelist)
        {
            return query.Where(i => nameWhitelist.ExactMatch(i.Naam));
        }

        public static IEnumerable<ResultRow> WhereWhitelistedCity(
            this IEnumerable<ResultRow> query, IWhitelistFilter nameWhitelist)
        {
            return query.Where(i => nameWhitelist.ExactMatch(i.City));
        }

        public static IEnumerable<ResultRow> WhereWhitelistedCLub(
            this IEnumerable<ResultRow> query, IWhitelistFilter nameWhitelist)
        {
            return query.Where(i => nameWhitelist.ExactMatch(i.Club));
        }
    }
}
