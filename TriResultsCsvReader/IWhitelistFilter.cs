namespace TriResultsCsvReader
{
    public interface IWhitelistFilter
    {
        bool ExactMatch(string name);

        bool StartsWithMatch(string name);

        bool ContainsMatch(string name);
    }
}