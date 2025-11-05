using System.Security.Cryptography;
using System.Text;

namespace FinanceZakatManager.Api.Utils;

public static class EtagGenerator
{
    public static string FromStrings(params string[] values)
    {
        using var sha = SHA256.Create();
        var input = string.Join('|', values);
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return $"\"{Convert.ToHexString(hash)}\"";
    }
}
