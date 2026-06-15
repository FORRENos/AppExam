using AppExam.Models;
using System.Collections.ObjectModel;

namespace AppExam.Services;

public sealed class DemoDataStore
{
    public ObservableCollection<Product> Products { get; } = new()
    {
        new Product
        {
            Id = 1,
            Article = "A112T4",
            Name = "Прокляты и убиты",
            UnitName = "шт.",
            Price = 585,
            SupplierName = "Виктор Астафьев",
            ManufacturerName = "Яуза",
            CategoryName = "Художественная литература",
            Discount = 25,
            QuantityInStock = 6,
            Description = "Роман-эпопея о военной прозе.",
            PhotoPath = "1.jpg"
        },
        new Product
        {
            Id = 2,
            Article = "G843H5",
            Name = "Тайны и загадки отца Брауна",
            UnitName = "шт.",
            Price = 193,
            SupplierName = "Гилберт Кит Честертон",
            ManufacturerName = "Яуза",
            CategoryName = "Художественная литература",
            Discount = 30,
            QuantityInStock = 9,
            Description = "Классические детективные рассказы.",
            PhotoPath = "2.jpg"
        },
        new Product
        {
            Id = 3,
            Article = "D325D4",
            Name = "Девайс",
            UnitName = "шт.",
            Price = 1599,
            SupplierName = "Кирилл Каланадзе",
            ManufacturerName = "Т8 Издательские технологии",
            CategoryName = "Художественная литература",
            Discount = 5,
            QuantityInStock = 12,
            Description = "Современный технологический роман.",
            PhotoPath = "3.jpg"
        },
        new Product
        {
            Id = 4,
            Article = "S432T5",
            Name = "Необыкновенное чудо. Школьные истории",
            UnitName = "шт.",
            Price = 549,
            SupplierName = "Людмила Улицкая",
            ManufacturerName = "Т8 Издательские технологии",
            CategoryName = "Художественная литература",
            Discount = 15,
            QuantityInStock = 15,
            Description = "Сборник рассказов.",
            PhotoPath = "4.jpg"
        },
        new Product
        {
            Id = 5,
            Article = "F325D4",
            Name = "Чук и Гек",
            UnitName = "шт.",
            Price = 209,
            SupplierName = "Аркадий Гайдар",
            ManufacturerName = "Т8 Издательские технологии",
            CategoryName = "Художественная литература",
            Discount = 18,
            QuantityInStock = 0,
            Description = "Детская повесть.",
            PhotoPath = "5.jpg"
        }
    };

    public IReadOnlyList<User> Users { get; } = new List<User>
    {
        new() { Id = 1, RoleName = "Администратор", FullName = "Никифорова Анна Семеновна", Login = "94d5ous@gmail.com", Password = "uzWC67" },
        new() { Id = 2, RoleName = "Менеджер", FullName = "Ситдикова Елена Анатольевна", Login = "ptec8ym@yahoo.com", Password = "LdNyos" },
        new() { Id = 3, RoleName = "Авторизованный клиент", FullName = "Степанов Михаил Артёмович", Login = "wpmrc3do@tutanota.com", Password = "RSbvHv" }
    };

    public User? Login(string login, string password)
    {
        return Users.FirstOrDefault(user =>
            user.Login.Equals(login.Trim(), StringComparison.OrdinalIgnoreCase)
            && user.Password == password);
    }

    public int GetNextProductId()
    {
        return Products.Count == 0 ? 1 : Products.Max(product => product.Id) + 1;
    }
}
