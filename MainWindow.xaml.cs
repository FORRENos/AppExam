using AppExam.Models;
using AppExam.Services;
using AppExam.Windows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AppExam;

public partial class MainWindow : Window
{
    private readonly DemoDataStore dataStore = new();
    private readonly ICollectionView productsView;
    private User? currentUser;

    public MainWindow()
    {
        InitializeComponent();
        productsView = CollectionViewSource.GetDefaultView(dataStore.Products);
        productsView.Filter = ProductFilter;
        ProductsListView.ItemsSource = productsView;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var user = dataStore.Login(LoginTextBox.Text, PasswordBox.Password);
        if (user == null)
        {
            MessageBox.Show("Проверьте логин и пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        OpenMainPanel(user);
    }

    private void GuestButton_Click(object sender, RoutedEventArgs e)
    {
        OpenMainPanel(null);
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        currentUser = null;
        MainPanel.Visibility = Visibility.Collapsed;
        LoginPanel.Visibility = Visibility.Visible;
    }

    private void OpenMainPanel(User? user)
    {
        currentUser = user;
        CurrentUserTextBlock.Text = user == null
            ? "Гость"
            : $"{user.FullName} ({user.RoleName})";

        var canManageProducts = user?.CanManageProducts == true;
        AddProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
        EditProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
        DeleteProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;

        LoginPanel.Visibility = Visibility.Collapsed;
        MainPanel.Visibility = Visibility.Visible;
    }

    private bool ProductFilter(object item)
    {
        if (item is not Product product)
        {
            return false;
        }

        var query = SearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(query) && !ContainsText(product, query))
        {
            return false;
        }

        var discountRange = ((ComboBoxItem)DiscountComboBox.SelectedItem).Content.ToString();
        return discountRange switch
        {
            "0-12,99%" => product.Discount >= 0 && product.Discount < 13,
            "13-16,99%" => product.Discount >= 13 && product.Discount < 17,
            "17% и более" => product.Discount >= 17,
            _ => true
        };
    }

    private static bool ContainsText(Product product, string query)
    {
        return product.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
            || product.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
            || product.CategoryName.Contains(query, StringComparison.OrdinalIgnoreCase)
            || product.SupplierName.Contains(query, StringComparison.OrdinalIgnoreCase)
            || product.ManufacturerName.Contains(query, StringComparison.OrdinalIgnoreCase)
            || product.Article.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    private void FilterControl_Changed(object sender, EventArgs e)
    {
        if (productsView == null)
        {
            return;
        }

        productsView.SortDescriptions.Clear();
        var sortName = ((ComboBoxItem)SortComboBox.SelectedItem).Content.ToString();
        switch (sortName)
        {
            case "Цена по возрастанию":
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Ascending));
                break;
            case "Цена по убыванию":
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Descending));
                break;
            case "Остаток по возрастанию":
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.QuantityInStock), ListSortDirection.Ascending));
                break;
            case "Остаток по убыванию":
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.QuantityInStock), ListSortDirection.Descending));
                break;
        }

        productsView.Refresh();
    }

    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        var product = new Product { Id = dataStore.GetNextProductId(), Article = $"NEW{dataStore.GetNextProductId():000}" };
        var window = new ProductEditorWindow(product) { Owner = this };
        if (window.ShowDialog() == true)
        {
            dataStore.Products.Add(product);
            productsView.Refresh();
        }
    }

    private void EditProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsListView.SelectedItem is not Product product)
        {
            MessageBox.Show("Выберите товар для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var copy = CopyProduct(product);
        var window = new ProductEditorWindow(copy) { Owner = this };
        if (window.ShowDialog() == true)
        {
            ApplyProduct(product, copy);
            productsView.Refresh();
        }
    }

    private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsListView.SelectedItem is not Product product)
        {
            MessageBox.Show("Выберите товар для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show("Удалить выбранный товар?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            dataStore.Products.Remove(product);
        }
    }

    private static Product CopyProduct(Product product)
    {
        return new Product
        {
            Id = product.Id,
            Article = product.Article,
            Name = product.Name,
            CategoryName = product.CategoryName,
            Description = product.Description,
            ManufacturerName = product.ManufacturerName,
            SupplierName = product.SupplierName,
            Price = product.Price,
            UnitName = product.UnitName,
            QuantityInStock = product.QuantityInStock,
            Discount = product.Discount,
            PhotoPath = product.PhotoPath
        };
    }

    private static void ApplyProduct(Product target, Product source)
    {
        target.Name = source.Name;
        target.CategoryName = source.CategoryName;
        target.Description = source.Description;
        target.ManufacturerName = source.ManufacturerName;
        target.SupplierName = source.SupplierName;
        target.Price = source.Price;
        target.UnitName = source.UnitName;
        target.QuantityInStock = source.QuantityInStock;
        target.Discount = source.Discount;
        target.PhotoPath = source.PhotoPath;
    }
}
