using AppExam.Models;
using AppExam.Services;
using System.Windows;

namespace AppExam
{
    public partial class MainWindow : Window
    {
        private readonly IDataStore dataStore;

        public MainWindow()
        {
            InitializeComponent();
            dataStore = CreateDataStore();
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

            OpenCatalogWindow(user);
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCatalogWindow(null);
        }

        private void OpenCatalogWindow(User? user)
        {
            var window = new AdminWindow(dataStore, user) { Owner = this };
            Hide();
            window.ShowDialog();
            Show();
        }
    }
}
