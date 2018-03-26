using System;

namespace TriResultsCsvReader
{
    public class RaceFileData
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string OutputFile { get; set; }

        public string FullPath { get; set; }
    }
}
