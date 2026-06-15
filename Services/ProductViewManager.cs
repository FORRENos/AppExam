using AppExam.Models;
using System.ComponentModel;
using System.Windows.Data;

namespace AppExam.Services;

public sealed class ProductViewManager
{
    private readonly ICollectionView productsView;
    private readonly Func<string> getSearchText;
    private readonly Func<int> getDiscountIndex;
    private readonly Func<int> getSortIndex;

    public ProductViewManager(
        ICollectionView productsView,
        Func<string> getSearchText,
        Func<int> getDiscountIndex,
        Func<int> getSortIndex)
    {
        this.productsView = productsView;
        this.getSearchText = getSearchText;
        this.getDiscountIndex = getDiscountIndex;
        this.getSortIndex = getSortIndex;
        this.productsView.Filter = ProductFilter;
    }

    public void Refresh()
    {
        ApplySort();
        productsView.Refresh();
    }

    private bool ProductFilter(object item)
    {
        if (item is not Product product)
        {
            return false;
        }

        var query = getSearchText().Trim();
        if (!string.IsNullOrWhiteSpace(query) && !ContainsText(product, query))
        {
            return false;
        }

        // Индексы совпадают с порядком пунктов ComboBox в MainWindow.xaml.
        return getDiscountIndex() switch
        {
            1 => product.Discount >= 0 && product.Discount < 13,
            2 => product.Discount >= 13 && product.Discount < 17,
            3 => product.Discount >= 17,
            _ => true
        };
    }

    private void ApplySort()
    {
        productsView.SortDescriptions.Clear();

        switch (getSortIndex())
        {
            case 1:
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Ascending));
                break;
            case 2:
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.Price), ListSortDirection.Descending));
                break;
            case 3:
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.QuantityInStock), ListSortDirection.Ascending));
                break;
            case 4:
                productsView.SortDescriptions.Add(new SortDescription(nameof(Product.QuantityInStock), ListSortDirection.Descending));
                break;
        }
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
}
