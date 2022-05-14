using System.Collections.Concurrent;

namespace Atamai.Slice.Sample;

public class Database
{
    public readonly ConcurrentDictionary<string, byte[]> Users = new(StringComparer.OrdinalIgnoreCase);
    public readonly ConcurrentDictionary<string, string> TokenUser = new(StringComparer.OrdinalIgnoreCase);

    public Database()
    {
        Users["user"] = PasswordHasher.Hash("test");
    }
}