using System;
using System.Collections.Generic;

public class Library
{
    private string name;
    private string address;
    private List<string> books;

    public int BooksCount => books.Count;

    public Library(string name, string address)
    {
        this.name = name;
        this.address = address;
        this.books = new List<string>();
    }

    public void AddBook(string bookTitle)
    {
        books.Add(bookTitle);
        Console.WriteLine($"Книга \"{bookTitle}\" додана до бібліотеки.");
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Бібліотека: {name}, Адреса: {address}, Книг: {BooksCount}");
    }
}

partial class Program
{
    static void Main(string[] args)
    {
        Library lib = new Library("Центральна бібліотека", "м. Рівне, вул. Соборна, 12");
        lib.AddBook("Кобзар");
        lib.AddBook("Місто");
        lib.PrintInfo();
    }
}
