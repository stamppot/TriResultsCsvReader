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

        public string StartNr { get; set; }

        public string Naam { get; set; }

        public string Club { get; set; }

        public string City { get; set; }
        public int? PosCat { get; set; }

        public string Cat { get; set; }

        public string Swim { get; set; }

        public int? PosSwim { get; set; }

        public string T1 { get; set; }
        public int? PosT1 { get; set; }
        public int? PosAfterT1 { get; set; }

        public string Bike { get; set; }

        public int? PosBike { get; set; }

        public string AfterBike { get; set; }

        public int? PosAfterBike { get; set; }

        public string T2 { get; set; }
        public int? PosT2 { get; set; }
        public int? PosAfterT2 { get; set; }

        public string Run { get; set; }

        public int? PosRun { get; set; }


        //<!-- RBR -->

        public string Run1 { get; set; }

        public int? PosRun1 { get; set; }

        public string Run2 { get; set; }

        public int? PosRun2 { get; set; }

        //<!-- end RBR -->

        /*** Team fields ***/
        public int? TeamPoints { get; set; }

        public int? TeamTotalPoints { get; set; }

        public int? TeamRank { get; set; }

        public string Difference { get; set; }

        public string Total { get; set; }

        public string RaceDate { get; set; }

        public string Race { get; set; }

    }
}
