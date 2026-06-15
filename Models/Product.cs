using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace AppExam.Models;

public sealed class Product : INotifyPropertyChanged
{
    private string name = "";
    private string categoryName = "";
    private string description = "";
    private string manufacturerName = "";
    private string supplierName = "";
    private decimal price;
    private string unitName = "шт.";
    private int quantityInStock;
    private int discount;
    private string photoPath = "";

    public int Id { get; set; }
    public string Article { get; set; } = "";

    public string Name
    {
        get => name;
        set => SetField(ref name, value);
    }

    public string CategoryName
    {
        get => categoryName;
        set => SetField(ref categoryName, value);
    }

    public string Description
    {
        get => description;
        set => SetField(ref description, value);
    }

    public string ManufacturerName
    {
        get => manufacturerName;
        set => SetField(ref manufacturerName, value);
    }

    public string SupplierName
    {
        get => supplierName;
        set => SetField(ref supplierName, value);
    }

    public decimal Price
    {
        get => price;
        set
        {
            if (SetField(ref price, value))
            {
                OnPropertyChanged(nameof(FinalPrice));
                OnPropertyChanged(nameof(PriceText));
            }
        }
    }

    public string UnitName
    {
        get => unitName;
        set => SetField(ref unitName, value);
    }

    public int QuantityInStock
    {
        get => quantityInStock;
        set
        {
            if (SetField(ref quantityInStock, value))
            {
                OnPropertyChanged(nameof(RowBrush));
            }
        }
    }

    public int Discount
    {
        get => discount;
        set
        {
            if (SetField(ref discount, value))
            {
                OnPropertyChanged(nameof(FinalPrice));
                OnPropertyChanged(nameof(PriceText));
                OnPropertyChanged(nameof(RowBrush));
            }
        }
    }

    public string PhotoPath
    {
        get => photoPath;
        set => SetField(ref photoPath, value);
    }

    public decimal FinalPrice => Math.Round(Price * (100 - Discount) / 100, 2);

    public string PriceText => Discount > 0
        ? $"{Price:0.00} руб. -> {FinalPrice:0.00} руб."
        : $"{Price:0.00} руб.";

    public Brush RowBrush
    {
        get
        {
            if (QuantityInStock <= 0)
            {
                return new SolidColorBrush(Color.FromRgb(220, 220, 220));
            }

            return Discount > 25
                ? new SolidColorBrush(Color.FromRgb(35, 225, 239))
                : Brushes.White;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
