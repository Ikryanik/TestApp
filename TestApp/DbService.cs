using System.Data;
using System.Data.SqlClient;

namespace TestApp;

public static class DbService
{
    private static readonly string ConnectionString;
    private static readonly SqlConnection Connection;
    static DbService()
    {
        ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
        Connection = new SqlConnection(ConnectionString);
    }
    
    private static bool IsAvailable()
    {
        try
        {
            Connection.Open();
            Connection.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task CreateTable()
    {
        if (!IsAvailable()) return;

        await Connection.OpenAsync();
        await using var command = Connection.CreateCommand();
        command.CommandText = Consts.CreateTableCommand;
        await command.ExecuteNonQueryAsync();

        await Connection.DisposeAsync();
    }

    public static async Task CreateLine(Customer customer)
    {
        if (!IsAvailable()) return;

        await Connection.OpenAsync();
        await using var command = Connection.CreateCommand();
        command.CommandText = Consts.CreateLineCommand;
        command.Parameters.AddWithValue("@p1", customer.FirstName);
        command.Parameters.AddWithValue("@p2", customer.LastName);
        command.Parameters.AddWithValue("@p3", customer.Patronymic ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@p4", customer.Birthday);
        command.Parameters.AddWithValue("@p5", customer.Gender);
        await command.ExecuteNonQueryAsync();

        await Connection.DisposeAsync();
    }

    public static async Task<List<Customer>?> ShowTable()
    {
        if (!IsAvailable()) return null;

        await Connection.OpenAsync();
        await using var command = Connection.CreateCommand();
        command.CommandText = Consts.ShowTableCommand;

        var customers = new List<Customer>();

        await using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            customers.Add(new Customer
            {
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Patronymic = reader.GetNullableString("Patronymic"),
                GenderName = reader.GetString("Gender"),
                Birthday = reader.GetDateTime("Birthday"),
                Age = reader.GetInt32("Age")
            });
        }

        await Connection.DisposeAsync();

        return customers;
    }

    public static async Task FillTheTable()
    {
        using var copy = new SqlBulkCopy(ConnectionString);
        copy.DestinationTableName = "Customer";
        var i = 0;

        var customers = new Customer[1_000_100];

        while (i < 1000000)
        {
            var customer = new Customer
            {
                FirstName = RandomEntry.RandomString(),
                LastName = RandomEntry.RandomString(),
                Patronymic = RandomEntry.RandomString(),
                GenderName = RandomEntry.RandomGender(),
                Birthday = RandomEntry.RandomDay()
            };

            customers[i] = customer;

            i++;
        }

        while (i < 1000100)
        {
            var customer = new Customer
            {
                FirstName = "F" + RandomEntry.RandomString(),
                LastName = RandomEntry.RandomString(),
                Patronymic = RandomEntry.RandomString(),
                GenderName = "м",
                Birthday = RandomEntry.RandomDay()
            };

            customers[i] = customer;

            i++;
        }

        copy.ColumnMappings.Add(nameof(Customer.FirstName), "FirstName");
        copy.ColumnMappings.Add(nameof(Customer.LastName), "LastName");
        copy.ColumnMappings.Add(nameof(Customer.Patronymic), "Patronymic");
        copy.ColumnMappings.Add(nameof(Customer.Birthday), "Birthday");
        copy.ColumnMappings.Add(nameof(Customer.Gender), "Gender");

        await copy.WriteToServerAsync(ToDataTable(customers));
    }

    public static async Task FillTheTableBad()
    {
        if (!IsAvailable()) return;

        await Connection.OpenAsync();
        await using var command = Connection.CreateCommand();
        command.CommandText = Consts.CreateLineCommand;

        var i = 0;

        while (i < 1000000)
        {
            var customer = new Customer
            {
                FirstName = RandomEntry.RandomString(),
                LastName = RandomEntry.RandomString(),
                Patronymic = RandomEntry.RandomString(),
                GenderName = RandomEntry.RandomGender(),
                Birthday = RandomEntry.RandomDay()
            };

            command.Parameters.AddWithValue("@p1", customer.FirstName);
            command.Parameters.AddWithValue("@p2", customer.LastName);
            command.Parameters.AddWithValue("@p3", (object)customer.Patronymic ?? DBNull.Value);
            command.Parameters.AddWithValue("@p4", customer.Birthday);
            command.Parameters.AddWithValue("@p5", customer.Gender);
            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
            i++;
        }

        await Connection.DisposeAsync();
    }

    public static DataTable? ToDataTable(Customer[] customers)
    {
        if (customers.Length == 0) return null;

        var table = new DataTable();
        var customerTmp = customers[0];

        table.Columns.Add(nameof(Customer.FirstName), customerTmp.FirstName.GetType());
        table.Columns.Add(nameof(Customer.LastName), customerTmp.LastName.GetType());
        table.Columns.Add(nameof(Customer.Patronymic), customerTmp.Patronymic?.GetType() ?? typeof(string));
        table.Columns.Add(nameof(Customer.Birthday), customerTmp.Birthday.GetType());
        table.Columns.Add(nameof(Customer.Gender), customerTmp.Gender.GetType());

        foreach (var customer in customers)
        {
            table.Rows.Add(customer.FirstName, customer.LastName, customer.Patronymic, customer.Birthday, customer.Gender);
        }

        return table;
    }

    public static async Task<List<Customer>?> ShowEntriesManWithFirstF()
    {
        if (!IsAvailable()) return null;

        await Connection.OpenAsync();
        await using var command = Connection.CreateCommand();
        command.CommandText = Consts.ShowEntriesManWithFirstF;

        var customers = new List<Customer>();

        await using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            try
            {
                customers.Add(new Customer
                {
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Patronymic = reader.GetNullableString("Patronymic"),
                    GenderName = reader.GetString("Gender"),
                    Birthday = reader.GetDateTime("Birthday")
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        await Connection.DisposeAsync();

        return customers;
    }

    public static async Task AddIndexes()
    {
        if (!IsAvailable()) return;

        await Connection.OpenAsync();
        var transaction = Connection.BeginTransaction();

        await using var command = Connection.CreateCommand();
        command.Transaction = transaction;

        try
        {
            command.CommandText = Consts.CreateGenderIndex;
            await command.ExecuteNonQueryAsync();
            command.CommandText = Consts.CreateFirstNameIndex;
            await command.ExecuteNonQueryAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }
}