using System;

namespace TriResultsCsvReader
{
    public class InvalidCsvException : Exception
    {
        public InvalidCsvException() : base()
        {
        }

        public InvalidCsvException(string message) : base(message)
        {

        }
    }
}
