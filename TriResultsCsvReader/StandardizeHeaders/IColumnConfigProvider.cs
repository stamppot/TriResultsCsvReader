using System.Collections.Generic;

namespace TriResultsCsvReader.StandardizeHeaders
{
    public interface IColumnConfigProvider
    {
        IEnumerable<Column> Get();
    }
}