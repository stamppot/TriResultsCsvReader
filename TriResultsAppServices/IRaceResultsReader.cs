using System;
using System.Collections.Generic;

namespace TriResultsAppServices
{
    public interface IRaceResultsReader
    {
        IEnumerable<string> ReadRaceRows(string filename);
    }
}