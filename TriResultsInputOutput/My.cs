using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib;

namespace TriResultsInputOutput
{
    public static class My
    {
        public static Func<DbConnection> ConnectionFactory = () => new SqlConnection(ConnectionString.Connection);

        public static class ConnectionString
        {
            public static string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["tri_wordpress"].ConnectionString;

            public static object ConfigurationManager { get; private set; }
        }
    }
}
