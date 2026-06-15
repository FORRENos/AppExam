using AppExam.Models;

namespace AppExam.Services;

public static class ProductFactory
{
    public static Product CreateCopy(Product product)
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

    public static void ApplyValues(Product target, Product source)
    {
        target.Article = source.Article;
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
