using System;
using System.Collections.Generic;

// Інтерфейс для відтворення
public interface IPlayable
{
    void Play();
    int Duration { get; } // тривалість композиції у хвилинах
}

// Абстрактний клас інструментів
public abstract class Instrument : IPlayable
{
    public string Name { get; set; }
    public int Duration { get; protected set; }

    public Instrument(string name, int duration)
    {
        Name = name;
        Duration = duration;
    }

    public abstract void Tune(); // абстрактний метод

    public virtual void Play()
    {
        Console.WriteLine($"{Name} грає композицію ({Duration} хв).");
    }
}

// Реалізація: Гітара
public class Guitar : Instrument
{
    public int Strings { get; set; }

    public Guitar(string name, int duration, int strings) 
        : base(name, duration)
    {
        Strings = strings;
    }

    public override void Tune()
    {
        Console.WriteLine($"Налаштовуємо гітару з {Strings} струнами...");
    }
}

// Реалізація: Піаніно
public class Piano : Instrument
{
    public int Keys { get; set; }

    public Piano(string name, int duration, int keys) 
        : base(name, duration)
    {
        Keys = keys;
    }

    public override void Tune()
    {
        Console.WriteLine($"Налаштовуємо піаніно з {Keys} клавішами...");
    }
}

// Клас Концерт (композиція)
public class Concert
{
    private List<IPlayable> instruments = new List<IPlayable>();

    public void AddInstrument(IPlayable instrument)
    {
        instruments.Add(instrument);
    }

    public void Start()
    {
        Console.WriteLine("🎶 Початок концерту!");
        foreach (var inst in instruments)
        {
            inst.Play();
        }
    }

    public int TotalDuration()
    {
        int sum = 0;
        foreach (var inst in instruments)
            sum += inst.Duration;
        return sum;
    }

    public int CountCompositions() => instruments.Count;
}

// Демонстрація
class Program
{
    static void Main()
    {
        Guitar guitar = new Guitar("Fender Stratocaster", 5, 6);
        Piano piano = new Piano("Yamaha Grand", 7, 88);

        Concert concert = new Concert();
        concert.AddInstrument(guitar);
        concert.AddInstrument(piano);

        guitar.Tune();
        piano.Tune();

        concert.Start();

        Console.WriteLine($"Загальна тривалість концерту: {concert.TotalDuration()} хв.");
        Console.WriteLine($"Кількість композицій: {concert.CountCompositions()}");
    }
}
