using System.Security.Cryptography;
using System.Text;

namespace Atamai.Slice.Sample;

public static class PasswordHasher
{
    public static byte[] Hash(string password)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(password));
    }

    public static bool Compare(ReadOnlySpan<byte> hashedPassword, string password)
    {
        return hashedPassword.SequenceEqual(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    }
}