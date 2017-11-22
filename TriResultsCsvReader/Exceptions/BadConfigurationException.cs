using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public class BadConfigurationException : Exception
    {
        public BadConfigurationException() : base()
        {
        }

        public BadConfigurationException(string message) : base(message)
        {
            
        }
    }
}
