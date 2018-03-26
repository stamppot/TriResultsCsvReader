using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class CsvFormatException : Exception
    {
        public CsvFormatException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
