using AppExam.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

namespace AppExam.Services;

public sealed class SqlDataStore : IDataStore
{
    private const string ConnectionString =
        @"Server=(localdb)\AppExamSQL;Database=AppExamDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public ObservableCollection<Product> Products { get; } = new();

    public void LoadProducts()
    {
        Products.Clear();

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        const string sql = """
            SELECT p.Id, p.Article, p.Name, u.Name AS UnitName, p.Price,
                   s.Name AS SupplierName, m.Name AS ManufacturerName,
                   c.Name AS CategoryName, p.Discount, p.QuantityInStock,
                   p.Description, p.PhotoPath
            FROM Products p
            JOIN Units u ON p.UnitId = u.Id
            JOIN Suppliers s ON p.SupplierId = s.Id
            JOIN Manufacturers m ON p.ManufacturerId = m.Id
            JOIN Categories c ON p.CategoryId = c.Id
            ORDER BY p.Id;
            """;

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            Products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Article = reader.GetString(1),
                Name = reader.GetString(2),
                UnitName = reader.GetString(3),
                Price = reader.GetDecimal(4),
                SupplierName = reader.GetString(5),
                ManufacturerName = reader.GetString(6),
                CategoryName = reader.GetString(7),
                Discount = reader.GetInt32(8),
                QuantityInStock = reader.GetInt32(9),
                Description = reader.IsDBNull(10) ? "" : reader.GetString(10),
                PhotoPath = reader.IsDBNull(11) ? "" : reader.GetString(11)
            });
        }
    }

    public User? Login(string login, string password)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        const string sql = """
            SELECT TOP 1 u.Id, r.Name AS RoleName, u.FullName, u.Login, u.Password
            FROM Users u
            JOIN Roles r ON u.RoleId = r.Id
            WHERE u.Login = @Login AND u.Password = @Password;
            """;

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Login", login.Trim());
        command.Parameters.AddWithValue("@Password", password);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            RoleName = reader.GetString(1),
            FullName = reader.GetString(2),
            Login = reader.GetString(3),
            Password = reader.GetString(4)
        };
    }

    public int GetNextProductId()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand("SELECT ISNULL(MAX(Id), 0) + 1 FROM Products;", connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void AddProduct(Product product)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var unitId = GetOrCreateLookupId(connection, "Units", product.UnitName);
        var supplierId = GetOrCreateLookupId(connection, "Suppliers", product.SupplierName);
        var manufacturerId = GetOrCreateLookupId(connection, "Manufacturers", product.ManufacturerName);
        var categoryId = GetOrCreateLookupId(connection, "Categories", product.CategoryName);

        const string sql = """
            INSERT INTO Products
                (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId,
                 Discount, QuantityInStock, Description, PhotoPath)
            OUTPUT INSERTED.Id
            VALUES
                (@Article, @Name, @UnitId, @Price, @SupplierId, @ManufacturerId, @CategoryId,
                 @Discount, @QuantityInStock, @Description, @PhotoPath);
            """;

        using var command = CreateProductCommand(connection, sql, product, unitId, supplierId, manufacturerId, categoryId);
        product.Id = Convert.ToInt32(command.ExecuteScalar());
        Products.Add(product);
    }

    public void UpdateProduct(Product product)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var unitId = GetOrCreateLookupId(connection, "Units", product.UnitName);
        var supplierId = GetOrCreateLookupId(connection, "Suppliers", product.SupplierName);
        var manufacturerId = GetOrCreateLookupId(connection, "Manufacturers", product.ManufacturerName);
        var categoryId = GetOrCreateLookupId(connection, "Categories", product.CategoryName);

        const string sql = """
            UPDATE Products
            SET Article = @Article,
                Name = @Name,
                UnitId = @UnitId,
                Price = @Price,
                SupplierId = @SupplierId,
                ManufacturerId = @ManufacturerId,
                CategoryId = @CategoryId,
                Discount = @Discount,
                QuantityInStock = @QuantityInStock,
                Description = @Description,
                PhotoPath = @PhotoPath
            WHERE Id = @Id;
            """;

        using var command = CreateProductCommand(connection, sql, product, unitId, supplierId, manufacturerId, categoryId);
        command.Parameters.AddWithValue("@Id", product.Id);
        command.ExecuteNonQuery();
    }

    public void DeleteProduct(Product product)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        using var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id;", connection);
        command.Parameters.AddWithValue("@Id", product.Id);
        command.ExecuteNonQuery();
        Products.Remove(product);
    }

    private static SqlCommand CreateProductCommand(
        SqlConnection connection,
        string sql,
        Product product,
        int unitId,
        int supplierId,
        int manufacturerId,
        int categoryId)
    {
        var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Article", product.Article);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@UnitId", unitId);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@SupplierId", supplierId);
        command.Parameters.AddWithValue("@ManufacturerId", manufacturerId);
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        command.Parameters.AddWithValue("@Discount", product.Discount);
        command.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(product.Description) ? DBNull.Value : product.Description);
        command.Parameters.AddWithValue("@PhotoPath", string.IsNullOrWhiteSpace(product.PhotoPath) ? DBNull.Value : product.PhotoPath);
        return command;
    }

    private static int GetOrCreateLookupId(SqlConnection connection, string tableName, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Не указано";
        }

        // Имя таблицы выбирается только из кода, поэтому здесь нет пользовательского SQL.
        using var selectCommand = new SqlCommand($"SELECT Id FROM {tableName} WHERE Name = @Name;", connection);
        selectCommand.Parameters.AddWithValue("@Name", name.Trim());
        var existingId = selectCommand.ExecuteScalar();
        if (existingId != null)
        {
            return Convert.ToInt32(existingId);
        }

        using var insertCommand = new SqlCommand($"INSERT INTO {tableName} (Name) OUTPUT INSERTED.Id VALUES (@Name);", connection);
        insertCommand.Parameters.AddWithValue("@Name", name.Trim());
        return Convert.ToInt32(insertCommand.ExecuteScalar());
    }
}
