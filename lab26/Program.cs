using System;
using System.IO;
 
// ============================================================
//  ПАТЕРН: Factory Method — логери
// ============================================================
 
/// <summary>Інтерфейс логера.</summary>
public interface ILogger
{
    void Log(string message);
}
 
/// <summary>Логер, що виводить повідомлення в консоль.</summary>
public class ConsoleLogger : ILogger
{
    public void Log(string message) =>
        Console.WriteLine($"[ConsoleLogger] {DateTime.Now:HH:mm:ss} | {message}");
}
 
/// <summary>Логер, що записує повідомлення у файл log.txt.</summary>
public class FileLogger : ILogger
{
    private readonly string _filePath;
 
    public FileLogger(string filePath = "log.txt") => _filePath = filePath;
 
    public void Log(string message)
    {
        string line = $"[FileLogger] {DateTime.Now:HH:mm:ss} | {message}";
        File.AppendAllText(_filePath, line + Environment.NewLine);
        Console.WriteLine($"[FileLogger → {_filePath}] {message}");
    }
}
 
/// <summary>Абстрактна фабрика логерів.</summary>
public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger();
}
 
/// <summary>Фабрика консольних логерів.</summary>
public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new ConsoleLogger();
}
 
/// <summary>Фабрика файлових логерів.</summary>
public class FileLoggerFactory : LoggerFactory
{
    private readonly string _filePath;
    public FileLoggerFactory(string filePath = "log.txt") => _filePath = filePath;
    public override ILogger CreateLogger() => new FileLogger(_filePath);
}
 
// ============================================================
//  ПАТЕРН: Singleton — менеджер логерів
// ============================================================
 
/// <summary>
/// Singleton, що зберігає єдиний екземпляр логера й дозволяє
/// динамічно змінювати фабрику (а отже — тип логера).
/// </summary>
public sealed class LoggerManager
{
    private static LoggerManager? _instance;
    private static readonly object _lock = new();
 
    private LoggerFactory _factory;
    private ILogger _logger;
 
    private LoggerManager(LoggerFactory factory)
    {
        _factory = factory;
        _logger  = factory.CreateLogger();
    }
 
    /// <summary>Повертає єдиний екземпляр; якщо не існує — створює з переданою фабрикою.</summary>
    public static LoggerManager GetInstance(LoggerFactory? factory = null)
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    if (factory is null)
                        throw new InvalidOperationException(
                            "Перший виклик GetInstance() потребує фабрики.");
                    _instance = new LoggerManager(factory);
                }
            }
        }
        return _instance;
    }
 
    /// <summary>Змінює фабрику й одразу перестворює логер.</summary>
    public void SetFactory(LoggerFactory factory)
    {
        _factory = factory;
        _logger  = factory.CreateLogger();
        _logger.Log("Фабрику логерів змінено.");
    }
 
    /// <summary>Логує повідомлення через поточний логер.</summary>
    public void Log(string message) => _logger.Log(message);
 
    /// <summary>Скидає Singleton (для демонстраційних цілей між сценаріями).</summary>
    public static void Reset() { lock (_lock) { _instance = null; } }
}
 
// ============================================================
//  ПАТЕРН: Strategy — обробка даних
// ============================================================
 
/// <summary>Інтерфейс стратегії обробки даних.</summary>
public interface IDataProcessorStrategy
{
    string Process(string data);
}
 
/// <summary>Стратегія «шифрування» (Base64).</summary>
public class EncryptDataStrategy : IDataProcessorStrategy
{
    public string Process(string data)
    {
        string encoded = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(data));
        Console.WriteLine($"  [EncryptDataStrategy] Зашифровано: {encoded}");
        return encoded;
    }
}
 
/// <summary>Стратегія «стиснення» (реверс рядка як символічна демонстрація).</summary>
public class CompressDataStrategy : IDataProcessorStrategy
{
    public string Process(string data)
    {
        char[] chars = data.ToCharArray();
        Array.Reverse(chars);
        string compressed = new string(chars);
        Console.WriteLine($"  [CompressDataStrategy] Стиснено: {compressed}");
        return compressed;
    }
}
 
/// <summary>Контекст, що делегує обробку поточній стратегії.</summary>
public class DataContext
{
    private IDataProcessorStrategy _strategy;
 
    public DataContext(IDataProcessorStrategy strategy) => _strategy = strategy;
 
    /// <summary>Змінює стратегію обробки «на льоту».</summary>
    public void SetStrategy(IDataProcessorStrategy strategy) => _strategy = strategy;
 
    /// <summary>Виконує обробку та повертає результат.</summary>
    public string ExecuteProcessing(string data)
    {
        Console.WriteLine($"  [DataContext] Вхідні дані: {data}");
        return _strategy.Process(data);
    }
}
 
// ============================================================
//  ПАТЕРН: Observer — публікатор оброблених даних
// ============================================================
 
/// <summary>Аргументи події обробки даних.</summary>
public class DataProcessedEventArgs : EventArgs
{
    public string OriginalData  { get; }
    public string ProcessedData { get; }
 
    public DataProcessedEventArgs(string original, string processed)
    {
        OriginalData  = original;
        ProcessedData = processed;
    }
}
 
/// <summary>Публікатор: сповіщає підписників про завершення обробки.</summary>
public class DataPublisher
{
    public event EventHandler<DataProcessedEventArgs>? DataProcessed;
 
    public void PublishDataProcessed(string original, string processed)
    {
        Console.WriteLine("  [DataPublisher] Публікую подію DataProcessed…");
        DataProcessed?.Invoke(this, new DataProcessedEventArgs(original, processed));
    }
}
 
/// <summary>
/// Спостерігач: підписується на DataPublisher.DataProcessed і логує
/// результат через Singleton LoggerManager.
/// </summary>
public class ProcessingLoggerObserver
{
    private readonly DataPublisher _publisher;
 
    public ProcessingLoggerObserver(DataPublisher publisher)
    {
        _publisher = publisher;
        _publisher.DataProcessed += OnDataProcessed;
    }
 
    private void OnDataProcessed(object? sender, DataProcessedEventArgs e)
    {
        LoggerManager.GetInstance().Log(
            $"[Observer] Оригінал: '{e.OriginalData}' → Результат: '{e.ProcessedData}'");
    }
 
    /// <summary>Відписує спостерігача від події.</summary>
    public void Detach() => _publisher.DataProcessed -= OnDataProcessed;
}
 
// ============================================================
//  Точка входу — демонстраційні сценарії
// ============================================================
 
class Program
{
    static void PrintSeparator(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 60));
    }
 
    static void Main()
    {
        // ── Сценарій 1: Повна інтеграція ─────────────────────────
        PrintSeparator("СЦЕНАРІЙ 1: Повна інтеграція");
 
        // 1. Ініціалізуємо Singleton з ConsoleLoggerFactory
        LoggerManager.Reset();
        var manager = LoggerManager.GetInstance(new ConsoleLoggerFactory());
        manager.Log("Сценарій 1 розпочато.");
 
        // 2. DataContext з EncryptDataStrategy
        var context   = new DataContext(new EncryptDataStrategy());
        var publisher = new DataPublisher();
        var observer  = new ProcessingLoggerObserver(publisher);
 
        // 3. Обробка та публікація
        const string data1 = "Hello, Patterns!";
        string result1 = context.ExecuteProcessing(data1);
        publisher.PublishDataProcessed(data1, result1);
 
        manager.Log("Сценарій 1 завершено.");
 
        // ── Сценарій 2: Динамічна зміна логера ───────────────────
        PrintSeparator("СЦЕНАРІЙ 2: Динамічна зміна логера");
 
        manager.Log("До зміни фабрики (ConsoleLogger).");
 
        // Змінюємо фабрику на FileLoggerFactory
        manager.SetFactory(new FileLoggerFactory("lab25_log.txt"));
 
        const string data2 = "FileLogger Test";
        string result2 = context.ExecuteProcessing(data2);
        publisher.PublishDataProcessed(data2, result2);
 
        manager.Log("Сценарій 2 завершено (FileLogger).");
 
        // ── Сценарій 3: Динамічна зміна стратегії ────────────────
        PrintSeparator("СЦЕНАРІЙ 3: Динамічна зміна стратегії");
 
        // Повертаємо ConsoleLogger для зручного читання
        manager.SetFactory(new ConsoleLoggerFactory());
        manager.Log("Стратегію буде змінено на CompressDataStrategy.");
 
        // Змінюємо стратегію
        context.SetStrategy(new CompressDataStrategy());
 
        const string data3 = "Compress Me!";
        string result3 = context.ExecuteProcessing(data3);
        publisher.PublishDataProcessed(data3, result3);
 
        manager.Log("Сценарій 3 завершено (CompressDataStrategy).");
 
        // ── Завершення ────────────────────────────────────────────
        PrintSeparator("Всі сценарії виконано успішно");
        Console.WriteLine("  Файл lab25_log.txt містить записи Сценарію 2.");
        Console.WriteLine();
    }
}
