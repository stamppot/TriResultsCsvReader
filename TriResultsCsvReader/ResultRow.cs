using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class ResultRow
    {

        public int Pos { get; set; }

        public int StartNr { get; set; }

        public string Naam { get; set; }

        public string Club { get; set; }

        public string City { get; set; }
        public int PosCat { get; set; }

        public string Cat { get; set; }

        public string Swim { get; set; }

        public int PosSwim { get; set; }

        public string T1 { get; set; }
        public string Bike { get; set; }

        public int PosBike { get; set; }

        public string AfterBike { get; set; }

        public int PosAfterBike { get; set; }

        public string T2 { get; set; }

        public string Run { get; set; }

        public int PosRun { get; set; }


        //<!-- RBR -->

        public string Run1 { get; set; }

        public int PosRun1 { get; set; }

        public string Run2 { get; set; }

        public int PosRun2 { get; set; }

        //<!-- end RBR -->


        public string Total { get; set; }

    }
}
