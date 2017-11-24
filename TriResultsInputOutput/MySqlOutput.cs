using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriResultsCsvReader;
using Dapper;
using System.Data;

namespace TriResultsInputOutput
{
    public class MySqlOutput
    {
        private string _connectionString;

        public MySqlOutput(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string Output(IEnumerable<ResultRow> resultRows)
        {
            var insertSql = @"INSERT INTO Results (Pos,StartNr,Naam,Club,City,PosCat,Cat,Swim,PosSwim,T1,PosT1,PosAfterT1,Bike,PosBike,AfterBike,PosAfterBike,T2,PosT2,PosAfterT2,Run,PosRun,Run1,PosRun1,Run2,PosRun2,TeamPoints,TeamTotalPoints,TeamRank,Difference,Total,RaceDate,Race) 
                              VALUES (@Pos,@StartNr,@Naam,@Club,@City,@PosCat,@Cat,@Swim,@PosSwim,@T1,@PosT1,@PosAfterT1,@Bike,@PosBike,@AfterBike,@PosAfterBike,@T2,@PosT2,@PosAfterT2,@Run,@PosRun,@Run1,@PosRun1,@Run2,@PosRun2,@TeamPoints,@TeamTotalPoints,@TeamRank,@Difference,@Total,@RaceDate,@Race)";

            int numRows = 0;
            using(var connection = My.ConnectionFactory())
            {
                connection.Open();

                numRows = connection.Execute(insertSql, resultRows);

            }

            return numRows.ToString();
        }
    }
}
