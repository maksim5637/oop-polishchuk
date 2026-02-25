public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[Console] {message}");
}

public class FileLogger : ILogger
{
    private readonly string filePath = "log.txt";
    public void Log(string message) => File.AppendAllText(filePath, $"[File] {message}\n");
}

public abstract class LoggerFactory
{
    public abstract ILogger CreateLogger();
}

public class ConsoleLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new ConsoleLogger();
}

public class FileLoggerFactory : LoggerFactory
{
    public override ILogger CreateLogger() => new FileLogger();
}

public class LoggerManager
{
    private static LoggerManager? instance;
    private LoggerFactory factory;

    private LoggerManager(LoggerFactory factory) => this.factory = factory;

    public static LoggerManager GetInstance(LoggerFactory factory)
    {
        if (instance == null)
            instance = new LoggerManager(factory);
        return instance;
    }

    public void SetFactory(LoggerFactory factory) => this.factory = factory;

    public void Log(string message) => factory.CreateLogger().Log(message);
}
public interface IDataProcessorStrategy
{
    string Process(string data);
}

public class EncryptDataStrategy : IDataProcessorStrategy
{
    public string Process(string data) => $"[Encrypted] {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data))}";
}

public class CompressDataStrategy : IDataProcessorStrategy
{
    public string Process(string data) => $"[Compressed] {string.Join("", data.Distinct())}";
}

public class DataContext
{
    private IDataProcessorStrategy strategy;

    public DataContext(IDataProcessorStrategy strategy) => this.strategy = strategy;

    public void SetStrategy(IDataProcessorStrategy strategy) => this.strategy = strategy;

    public string Execute(string data) => strategy.Process(data);
}
public class DataPublisher
{
    public event Action<string>? DataProcessed;

    public void PublishDataProcessed(string data) => DataProcessed?.Invoke(data);
}

public class ProcessingLoggerObserver
{
    public ProcessingLoggerObserver(DataPublisher publisher)
    {
        publisher.DataProcessed += OnDataProcessed;
    }

    private void OnDataProcessed(string data)
    {
        LoggerManager.GetInstance(new ConsoleLoggerFactory()).Log($"Observer received processed data: {data}");
    }
}
partial class Program
{
    static async Task Main(string[] args)
    {
        string rawData = "Hello World";

        Console.WriteLine("===== Сценарій 1: Повна інтеграція =====");
        var loggerManager = LoggerManager.GetInstance(new ConsoleLoggerFactory());
        var context = new DataContext(new EncryptDataStrategy());
        var publisher = new DataPublisher();
        var observer = new ProcessingLoggerObserver(publisher);

        string processed = context.Execute(rawData);
        loggerManager.Log($"Data processed: {processed}");
        publisher.PublishDataProcessed(processed);

        // Сценарій 2: Динамічна зміна логера
        Console.WriteLine("\n===== Сценарій 2: Динамічна зміна логера =====");
        loggerManager.SetFactory(new FileLoggerFactory());
        processed = context.Execute(rawData);
        loggerManager.Log($"Data processed: {processed}");
        publisher.PublishDataProcessed(processed);

        // Сценарій 3: Динамічна зміна стратегії
        Console.WriteLine("\n===== Сценарій 3: Динамічна зміна стратегії =====");
        context.SetStrategy(new CompressDataStrategy());
        processed = context.Execute(rawData);
        loggerManager.Log($"Data processed: {processed}");
        publisher.PublishDataProcessed(processed);

        Console.WriteLine("\n===== Кінець демонстрації =====");
    }
}