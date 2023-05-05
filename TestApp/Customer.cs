namespace TestApp;

public class Customer
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public DateTime Birthday { get; set; } = DateTime.Now;
    public string GenderName { get; set; } = string.Empty;
    public char Gender => GenderName switch
    {
        "женский" => 'ж',
        "ж" => 'ж',
        "мужской" => 'м',
        "м" => 'м',
        _ => 'x'
    };
    public int Age { get; set; }
}