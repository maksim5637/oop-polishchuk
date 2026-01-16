using System;
using System.Collections.Generic;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString() => $"{Name} - {Price} грн";
}

public class ShopCart
{
    private List<Product> products = new List<Product>();

    // Властивість для кількості товарів
    public int Count => products.Count;

    // Індексатор за індексом
    public Product this[int index]
    {
        get => products[index];
        set => products[index] = value;
    }

    // Індексатор за назвою
    public Product this[string name]
    {
        get => products.Find(p => p.Name == name);
    }

    // Оператор додавання товару
    public static ShopCart operator +(ShopCart cart, Product product)
    {
        cart.products.Add(product);
        return cart;
    }

    // Оператор видалення товару
    public static ShopCart operator -(ShopCart cart, Product product)
    {
        cart.products.RemoveAll(p => p.Name == product.Name);
        return cart;
    }

    // Для зручності – вивід усіх товарів
    public void ShowCart()
    {
        Console.WriteLine("Товари у кошику:");
        foreach (var p in products)
            Console.WriteLine(p);
    }
}
