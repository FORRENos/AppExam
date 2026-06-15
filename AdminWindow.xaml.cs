using AppExam.Models;
using AppExam.Services;
using AppExam.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace AppExam
{
    public partial class AdminWindow : Window
    {
        private readonly IDataStore dataStore;
        private readonly ICollectionView productsView;
        private readonly ProductViewManager productViewManager;
        private readonly User? currentUser;

        public AdminWindow(IDataStore dataStore, User? currentUser)
        {
            InitializeComponent();

            this.dataStore = dataStore;
            this.currentUser = currentUser;

            productsView = CollectionViewSource.GetDefaultView(dataStore.Products);
            productViewManager = new ProductViewManager(
                productsView,
                () => SearchTextBox.Text,
                () => DiscountComboBox.SelectedIndex,
                () => SortComboBox.SelectedIndex);

            ProductsDataGrid.ItemsSource = productsView;
            PrepareUserInterface();
        }

        private void PrepareUserInterface()
        {
            UserNameTextBlock.Text = currentUser == null
                ? "Гость"
                : $"{currentUser.FullName} ({currentUser.RoleName})";

            // Добавлять, менять и удалять товары может только администратор.
            var canManageProducts = currentUser?.CanManageProducts == true;
            AddProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
            EditProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
            DeleteProductButton.Visibility = canManageProducts ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FilterControl_Changed(object sender, EventArgs e)
        {
            if (productViewManager == null)
            {
                return;
            }

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
            if (ProductsDataGrid.SelectedItem is not Product product)
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
            if (ProductsDataGrid.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Удалить выбранный товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                dataStore.DeleteProduct(product);
                productViewManager.Refresh();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
