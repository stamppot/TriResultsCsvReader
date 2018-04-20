using System;
using System.Collections.Generic;

namespace TriResultsCsvReader
{
    // an envelope to pass race message in
    // metadata about the race

    public class RaceEnvelope
    {
        public RaceEnvelope()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            RaceData = new Race();
        }

        public Guid Id { get; private set; }

        /// <summary>
        /// Creation time in UTC
        /// </summary>
        public DateTime Timestamp { get; private set; }

        public Race RaceData { get; set; }

        public string InputFile { get; set; }
        public string FullPath { get; set; }  // one of these should be made redundant

        public List<string> OutputOptions { get; set; }
        
        public string OutputFolder { get; set; }

        public IEnumerable<Column> ColumnConfig { get; set; }
    }
}
