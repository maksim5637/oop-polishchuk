using System;

namespace Lab21
{
    // ====== Інтерфейс стратегії доставки ======
    public interface IShippingStrategy
    {
        decimal CalculateCost(decimal distance, decimal weight);
    }

    // ====== Реалізації стратегій доставки ======
    public class StandardShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
            => distance * 1.5m + weight * 0.5m;
    }

    public class ExpressShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
            => (distance * 2.5m + weight * 1.0m) + 50m;
    }

    public class InternationalShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            decimal baseCost = distance * 5.0m + weight * 2.0m;
            return baseCost * 1.15m; // +15% податок
        }
    }

    public class NightShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
            => (distance * 1.5m + weight * 0.5m) + 30m; // націнка за нічну доставку
    }

    // ====== Factory Method для доставки ======
    public static class ShippingStrategyFactory
    {
        public static IShippingStrategy CreateStrategy(string deliveryType)
        {
            return deliveryType.ToLower() switch
            {
                "standard" => new StandardShippingStrategy(),
                "express" => new ExpressShippingStrategy(),
                "international" => new InternationalShippingStrategy(),
                "night" => new NightShippingStrategy(),
                _ => throw new ArgumentException("Unknown delivery type")
            };
        }
    }

    // ====== DeliveryService ======
    public class DeliveryService
    {
        public decimal CalculateDeliveryCost(decimal distance, decimal weight, IShippingStrategy strategy)
            => strategy.CalculateCost(distance, weight);
    }

    // ====== Інтерфейс стратегії конвертації ======
    public interface IExchangeStrategy
    {
        decimal Exchange(decimal amount);
    }

    // ====== Реалізації стратегій конвертації ======
    public class StandardExchange : IExchangeStrategy
    {
        public decimal Exchange(decimal amount)
            => amount - (amount * 0.02m + 5m); // 2% + 5 грн комісія
    }

    public class CardExchange : IExchangeStrategy
    {
        public decimal Exchange(decimal amount)
            => amount - (amount * 0.03m + 10m); // 3% + 10 грн комісія
    }

    public class CryptoExchange : IExchangeStrategy
    {
        public decimal Exchange(decimal amount)
            => amount - (amount * 0.05m + 2m); // 5% + 2 грн комісія
    }

    // ====== Factory Method для конвертації ======
    public static class ExchangeStrategyFactory
    {
        public static IExchangeStrategy CreateStrategy(string type)
        {
            return type.ToLower() switch
            {
                "standard" => new StandardExchange(),
                "card" => new CardExchange(),
                "crypto" => new CryptoExchange(),
                _ => throw new ArgumentException("Unknown exchange type")
            };
        }
    }

    // ====== Main ======
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Shipping Calculator ===");
            Console.Write("Enter delivery type (Standard/Express/International/Night): ");
            string type = Console.ReadLine();

            Console.Write("Enter distance (km): ");
            decimal distance = decimal.Parse(Console.ReadLine());

            Console.Write("Enter weight (kg): ");
            decimal weight = decimal.Parse(Console.ReadLine());

            IShippingStrategy strategy = ShippingStrategyFactory.CreateStrategy(type);
            DeliveryService service = new DeliveryService();
            decimal cost = service.CalculateDeliveryCost(distance, weight, strategy);

            Console.WriteLine($"Delivery cost: {cost} UAH");

            Console.WriteLine("\n=== Currency Exchange ===");
            Console.Write("Enter exchange type (Standard/Card/Crypto): ");
            string exchangeType = Console.ReadLine();

            Console.Write("Enter amount: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            IExchangeStrategy exchangeStrategy = ExchangeStrategyFactory.CreateStrategy(exchangeType);
            decimal result = exchangeStrategy.Exchange(amount);

            Console.WriteLine($"Amount after commission: {result} UAH");
        }
    }
}
