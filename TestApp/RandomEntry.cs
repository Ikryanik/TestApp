namespace TestApp;

public static class RandomEntry
{
    private static readonly Random Random = new();
    public static string RandomString()
    {
        var length = Random.Next(1, 15);
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static DateTime RandomDay()
    {
        var start = new DateTime(1920, 1, 1);
        var range = (DateTime.Today - start).Days;
        return start.AddDays(Random.Next(range));
    }

    public static string RandomGender()
    {
        var result = Random.Next(2);
        return result == 0 ? "ж" : "м";
    }
}