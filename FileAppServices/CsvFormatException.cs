using System;

namespace FileAppServices
{
    public class CsvFormatException : Exception
    {
        public CsvFormatException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
