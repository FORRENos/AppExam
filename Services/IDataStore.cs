using AppExam.Models;
using System.Collections.ObjectModel;

namespace AppExam.Services;

public interface IDataStore
{
    ObservableCollection<Product> Products { get; }

    User? Login(string login, string password);

    int GetNextProductId();

    void LoadProducts();

    void AddProduct(Product product);

    void UpdateProduct(Product product);

    void DeleteProduct(Product product);
}
