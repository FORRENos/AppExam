using AppExam.Models;
using AppExam.Services;
using AppExam.Windows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace AppExam;

public partial class MainWindow : Window
{
    private readonly IDataStore dataStore;
    private readonly ICollectionView productsView;
    private readonly ProductViewManager productViewManager;
    private User? currentUser;

    public MainWindow()
    {
        InitializeComponent();

        dataStore = CreateDataStore();
        productsView = CollectionViewSource.GetDefaultView(dataStore.Products);

        // Отдельный класс отвечает за поиск, фильтр и сортировку списка товаров.
        productViewManager = new ProductViewManager(
            productsView,
            () => SearchTextBox.Text,
            () => DiscountComboBox.SelectedIndex,
            () => SortComboBox.SelectedIndex);

        ProductsListView.ItemsSource = productsView;
    }

    private static IDataStore CreateDataStore()
    {
        try
        {
            var sqlDataStore = new SqlDataStore();
            sqlDataStore.LoadProducts();
            return sqlDataStore;
        }
        catch (Exception)
        {
            MessageBox.Show(
                "База данных не найдена. Приложение запущено на демо-данных.",
                "Подключение к БД",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var demoDataStore = new DemoDataStore();
            demoDataStore.LoadProducts();
            return demoDataStore;
        }
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

        // Управление товарами доступно только администратору.
        var canManageProducts = user?.CanManageProducts == true;
        AddProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
        EditProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
        DeleteProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;

        LoginPanel.Visibility = Visibility.Collapsed;
        MainPanel.Visibility = Visibility.Visible;
    }

    private void FilterControl_Changed(object sender, EventArgs e)
    {
        productViewManager.Refresh();
    }

    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        var product = new Product
        {
            Id = dataStore.GetNextProductId(),
            Article = $"NEW{dataStore.GetNextProductId():000}"
        };

        var window = new ProductEditorWindow(product) { Owner = this };
        if (window.ShowDialog() == true)
        {
            dataStore.AddProduct(product);
            productViewManager.Refresh();
        }
    }

    private void EditProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsListView.SelectedItem is not Product product)
        {
            MessageBox.Show("Выберите товар для редактирования.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var copy = ProductFactory.CreateCopy(product);
        var window = new ProductEditorWindow(copy) { Owner = this };
        if (window.ShowDialog() == true)
        {
            ProductFactory.ApplyValues(product, copy);
            dataStore.UpdateProduct(product);
            productViewManager.Refresh();
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
            dataStore.DeleteProduct(product);
        }
    }
}
