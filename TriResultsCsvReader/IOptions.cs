using System;


namespace TriResultsCsvReader
{
    public interface IOptions
    {

        string MemberFile { get;  }

        string FilterKeywords { get;  }

        int FilterYear { get; set; }

        string ConfigFile { get;  }

        string InputFolderOrFile { get; }

        string OutputFolder { get; set; }

        bool OutputSql { get; set; }

        bool OutputCsv { get; set; }

        bool OutputHtml { get; set; }

        
        bool Verbose { get; set; }

        DateTime RaceDate { get;  }

        string RaceName { get; }

        
    }
}
