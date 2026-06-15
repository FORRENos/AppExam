using AppExam.Models;
using System.Globalization;
using System.Windows;

namespace AppExam.Windows;

public partial class ProductEditorWindow : Window
{
    private readonly Product product;

    public ProductEditorWindow(Product product)
    {
        InitializeComponent();
        this.product = product;
        LoadProduct();
    }

    private void LoadProduct()
    {
        IdTextBox.Text = product.Id.ToString(CultureInfo.InvariantCulture);
        ArticleTextBox.Text = product.Article;
        NameTextBox.Text = product.Name;
        CategoryTextBox.Text = product.CategoryName;
        DescriptionTextBox.Text = product.Description;
        ManufacturerTextBox.Text = product.ManufacturerName;
        SupplierTextBox.Text = product.SupplierName;
        PriceTextBox.Text = product.Price.ToString("0.00", CultureInfo.CurrentCulture);
        UnitTextBox.Text = product.UnitName;
        QuantityTextBox.Text = product.QuantityInStock.ToString(CultureInfo.InvariantCulture);
        DiscountTextBox.Text = product.Discount.ToString(CultureInfo.InvariantCulture);
        PhotoTextBox.Text = product.PhotoPath;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput(out var price, out var quantity, out var discount))
        {
            return;
        }

        product.Article = ArticleTextBox.Text.Trim();
        product.Name = NameTextBox.Text.Trim();
        product.CategoryName = CategoryTextBox.Text.Trim();
        product.Description = DescriptionTextBox.Text.Trim();
        product.ManufacturerName = ManufacturerTextBox.Text.Trim();
        product.SupplierName = SupplierTextBox.Text.Trim();
        product.Price = price;
        product.UnitName = UnitTextBox.Text.Trim();
        product.QuantityInStock = quantity;
        product.Discount = discount;
        product.PhotoPath = PhotoTextBox.Text.Trim();

        DialogResult = true;
    }

    private bool ValidateInput(out decimal price, out int quantity, out int discount)
    {
        price = 0;
        quantity = 0;
        discount = 0;

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            ShowValidationMessage("Заполните наименование товара.");
            return false;
        }

        if (!decimal.TryParse(PriceTextBox.Text, out price) || price < 0)
        {
            ShowValidationMessage("Стоимость товара должна быть числом и не может быть отрицательной.");
            return false;
        }

        if (!int.TryParse(QuantityTextBox.Text, out quantity) || quantity < 0)
        {
            ShowValidationMessage("Количество на складе не может быть отрицательным.");
            return false;
        }

        if (!int.TryParse(DiscountTextBox.Text, out discount) || discount < 0 || discount > 100)
        {
            ShowValidationMessage("Скидка должна быть числом от 0 до 100.");
            return false;
        }

        return true;
    }

    private static void ShowValidationMessage(string message)
    {
        MessageBox.Show(message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
