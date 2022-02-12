using System.Collections.Concurrent;

namespace Atamai.Slice.Sample;

public class DataBase
{
    public readonly ConcurrentDictionary<string, byte[]> Users = new(StringComparer.OrdinalIgnoreCase);
    public readonly ConcurrentDictionary<string, string> TokenUser = new(StringComparer.OrdinalIgnoreCase);

    public DataBase()
    {
        Users["user"] = PasswordHasher.Hash("test");
    }
}