namespace TestApp;

public static class Consts
{
    public static string CreateTableCommand = @"DROP TABLE IF EXISTS Customer;                                                
	                                            CREATE TABLE Customer(
	                                            Id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	                                            FirstName nvarchar(50) NOT NULL,
	                                            LastName nvarchar(50) NOT NULL,
	                                            Patronymic nvarchar(50),
	                                            Birthday date NOT NULL,
	                                            Gender char(1) NOT NULL)";

    public static string CreateLineCommand = @"INSERT INTO Customer (FirstName, LastName, Patronymic, Birthday, Gender)
                                               VALUES (@p1, @p2, @p3, @p4, @p5);";

    public static string ShowTableCommand = @"SELECT FirstName, LastName, Patronymic, Birthday, Gender, 
                                            DATEDIFF(year, Birthday, getdate()) + CASE WHEN (DATEADD(year,DATEDIFF(year, Birthday, getdate()) , Birthday) > getdate()) 
                                            THEN - 1 
                                            ELSE 0 
                                            END as Age 
                                            FROM Customer
                                            WHERE Id in 
                                            (SELECT Min(Id) FROM Customer
                                            GROUP BY FirstName, LastName, Patronymic, Birthday)
                                            ORDER BY FirstName, LastName, Patronymic;";

    public static string ShowEntriesManWithFirstF = @"SELECT * FROM Customer 
                                                      WHERE Gender = 'м' AND FirstName LIKE ('F%');";

    public static string CreateGenderIndex = @"CREATE INDEX Index_Gender ON Customer (Gender);";
    public static string CreateFirstNameIndex = @"CREATE INDEX Index_FirstName ON Customer (FirstName);";
}