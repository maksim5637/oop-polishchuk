using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        // Колекція чисел
        List<int> nums = new() { 1, 2, 3, 4, 5, 6, 7, 11, 15 };

        // 🔹 1. Анонімний метод (delegate keyword)
        Func<int, int, bool> isPrime = delegate (int number, int dummy)
        {
            if (number < 2) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
                if (number % i == 0) return false;
            return true;
        };

        Console.WriteLine("Перевірка простих чисел (анонімний метод):");
        foreach (var n in nums)
            Console.WriteLine($"{n} → {isPrime(n, 0)}");

        // 🔹 2. Лямбда-вираз (короткий запис)
        Func<int, int, bool> isPrimeLambda = (number, _) =>
        {
            if (number < 2) return false;
            return !Enumerable.Range(2, (int)Math.Sqrt(number) - 1)
                              .Any(i => number % i == 0);
        };

        Console.WriteLine("\nПеревірка простих чисел (лямбда):");
        nums.ForEach(n => Console.WriteLine($"{n} → {isPrimeLambda(n, 0)}"));

        // 🔹 3. Action<List<int>> — друк списку
        Action<List<int>> printList = list =>
        {
            Console.WriteLine("\nДрук списку:");
            list.ForEach(x => Console.Write($"{x} "));
            Console.WriteLine();
        };
        printList(nums);

        // 🔹 4. Predicate<int> — видалення непотрібних елементів (наприклад, парних)
        Predicate<int> removeEven = n => n % 2 == 0;
        nums.RemoveAll(removeEven);
        Console.WriteLine("\nСписок після видалення парних чисел:");
        nums.ForEach(x => Console.Write($"{x} "));
        Console.WriteLine();

        // 🔹 5. LINQ-операції
        var squared = nums.Select(x => x * x); // Select
        var ordered = squared.OrderBy(x => x); // OrderBy
        var sum = ordered.Aggregate((acc, val) => acc + val); // Aggregate

        Console.WriteLine("\nКвадрати чисел:");
        foreach (var s in squared) Console.Write($"{s} ");
        Console.WriteLine("\nВідсортовані квадрати:");
        foreach (var o in ordered) Console.Write($"{o} ");
        Console.WriteLine($"\nСума квадратів: {sum}");
    }
}