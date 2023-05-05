using System.Data;
using System.Data.Common;

namespace TestApp;

public static class Extensions
{
    public static string? GetNullableString(this DbDataReader reader, string name)
    {
        return reader.IsDBNull(name) ? null : reader.GetString(name);
    }
}