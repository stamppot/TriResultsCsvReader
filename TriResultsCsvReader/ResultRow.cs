using System;

namespace TriResultsCsvReader
{
    public class ResultRow
    {

        public int Pos { get; set; }

        public string StartNr { get; set; }

        public string Naam { get; set; }

        public string RaceType { get; set; }

        public string Club { get; set; }

        public string City { get; set; }
        public int? PosCat { get; set; }

        public string Cat { get; set; }

        public string Swim { get; set; }

        public string SwimPace { get; set; }

        public int? PosSwim { get; set; }


        public string Run1 { get; set; }

        public int? PosRun1 { get; set; }

        public string T1 { get; set; }
        public int? PosT1 { get; set; }
        public int? PosAfterT1 { get; set; }

        public string Bike { get; set; }

        public string BikePace { get; set; }

        public int? PosBike { get; set; }

        public string AfterBike { get; set; }

        public int? PosAfterBike { get; set; }

        public string T2 { get; set; }
        public int? PosT2 { get; set; }
        public int? PosAfterT2 { get; set; }

        public string Run { get; set; }

        public string RunPace { get; set; }

        public int? PosRun { get; set; }


        //<!-- RBR -->


        public string Run2 { get; set; }

        public int? PosRun2 { get; set; }

        //<!-- end RBR -->

        public string Distance { get; set; }
        /*** Team fields ***/
        public int? TeamPoints { get; set; }

        public int? TeamNr { get; set; }

        public int? TeamTotalPoints { get; set; }

        public string TeamTotal { get; set; }

        public int? TeamRank { get; set; }

        public string Difference { get; set; }

        public string Total { get; set; }
        
        public string Race { get; set; }

        public DateTime RaceDate { get; set; }



        public object GetPropertyValue(string name)
        {
            var prop = this.GetType().GetProperty(name);
            if(prop != null)
            {
                return prop.GetValue(this, null);
            }
            return null;
        }

        public PropertyType GetPropertyType(string name)
        {
            var prop = this.GetType().GetProperty(name);
            var propType = prop.PropertyType;
            if (propType.Name == "String")
            {
                return PropertyType.aString;
            }
            if(propType.Name == "Int32")
            {
                return PropertyType.anInt;
            }
            if (propType.Name.StartsWith("Nullable") && propType.FullName.Contains("Int32"))
            {
                return PropertyType.aNullableInt;
            }
            if (propType.Name == "DateTime")
            {
                return PropertyType.aDate;
            }
            
            return PropertyType.aString;
        }
    }
}
