using CsvHelper.Configuration.Attributes;

namespace TriResultsCsvReader
{
    public class MemberRow
    {
        [Name("Naam")]
        public string Name { get; set; }

        [Name("Lidsinds")]
        public string MemberSince { get; set; }

        [Name("Geslacht")]
        public string Gender { get; set; }
    }
}
