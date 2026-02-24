using System;
using System.Collections.Generic;

namespace lab24
{
    public interface INumericOperationStrategy
    {
        double Execute(double value);
        string Name { get; }
    }

    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => value * value;
        public string Name => "Square";
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => value * value * value;
        public string Name => "Cube";
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => Math.Sqrt(value);
        public string Name => "Square Root";
    }

    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;

        public NumericProcessor(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
        }

        public double Process(double input)
        {
            return _strategy.Execute(input);
        }

        public string CurrentOperationName => _strategy.Name;
    }

    public class ResultPublisher
    {
        public event Action<double, string> ResultCalculated = delegate { };

        public void PublishResult(double result, string operationName)
        {
            ResultCalculated?.Invoke(result, operationName);
        }
    }

    public class ConsoleLoggerObserver
    {
        public void OnResultCalculated(double result, string operationName)
        {
            Console.WriteLine($"[ConsoleLogger] Operation: {operationName}, Result: {result}");
        }
    }

    public class HistoryLoggerObserver
    {
        private readonly List<string> _history = new List<string>();

        public void OnResultCalculated(double result, string operationName)
        {
            _history.Add($"{operationName}: {result}");
        }

        public void PrintHistory()
        {
            Console.WriteLine("\n[HistoryLogger] Calculation History:");
            foreach (var entry in _history)
            {
                Console.WriteLine(entry);
            }
        }
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;

        public ThresholdNotifierObserver(double threshold)
        {
            _threshold = threshold;
        }

        public void OnResultCalculated(double result, string operationName)
        {
            if (result > _threshold)
            {
                Console.WriteLine($"[ThresholdNotifier] Result {result} from {operationName} exceeds threshold {_threshold}!");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new NumericProcessor(new SquareOperationStrategy());
            var publisher = new ResultPublisher();

            var consoleLogger = new ConsoleLoggerObserver();
            var historyLogger = new HistoryLoggerObserver();
            var thresholdNotifier = new ThresholdNotifierObserver(50);

            publisher.ResultCalculated += consoleLogger.OnResultCalculated;
            publisher.ResultCalculated += historyLogger.OnResultCalculated;
            publisher.ResultCalculated += thresholdNotifier.OnResultCalculated;

            double[] numbers = { 4, 10, 25 };

            processor.SetStrategy(new SquareOperationStrategy());
            foreach (var num in numbers)
            {
                double result = processor.Process(num);
                publisher.PublishResult(result, processor.CurrentOperationName);
            }

            processor.SetStrategy(new CubeOperationStrategy());
            foreach (var num in numbers)
            {
                double result = processor.Process(num);
                publisher.PublishResult(result, processor.CurrentOperationName);
            }

            processor.SetStrategy(new SquareRootOperationStrategy());
            foreach (var num in numbers)
            {
                double result = processor.Process(num);
                publisher.PublishResult(result, processor.CurrentOperationName);
            }

            historyLogger.PrintHistory();

            Console.WriteLine("\nDemo finished.");
        }
    }
}
