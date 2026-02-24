using System;
using System.Collections.Concurrent;

#nullable enable

namespace OrdersApp
{
    // Модель і статуси
    public enum OrderStatus
    {
        New,
        Validated,
        Saved,
        Notified,
        Completed,
        Failed
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
        }
    }

    // --- Оригінальний OrderProcessor (до рефакторингу) ---
    public class OrderProcessor
    {
        public void ProcessOrder(Order order)
        {
            if (order == null)
            {
                Console.WriteLine("Order is null.");
                return;
            }

            Console.WriteLine($"[OldProcessor] Processing order #{order.Id} for {order.CustomerName}.");

            if (!Validate(order))
            {
                order.Status = OrderStatus.Failed;
                Console.WriteLine($"[OldProcessor] Order #{order.Id} validation failed. Status set to {order.Status}.");
                return;
            }
            order.Status = OrderStatus.Validated;
            Console.WriteLine($"[OldProcessor] Order #{order.Id} validated. Status set to {order.Status}.");

            SaveToDatabase(order);
            order.Status = OrderStatus.Saved;
            Console.WriteLine($"[OldProcessor] Order #{order.Id} saved. Status set to {order.Status}.");

            SendEmailNotification(order);
            order.Status = OrderStatus.Notified;
            Console.WriteLine($"[OldProcessor] Notification sent for order #{order.Id}. Status set to {order.Status}.");

            order.Status = OrderStatus.Completed;
            Console.WriteLine($"[OldProcessor] Order #{order.Id} processing completed. Final status {order.Status}.");
        }

        private bool Validate(Order order) => order.TotalAmount > 0;

        private void SaveToDatabase(Order order)
        {
            Console.WriteLine($"[OldProcessor][DB] Saving order #{order.Id} (Customer: {order.CustomerName}, Amount: {order.TotalAmount}).");
        }

        private void SendEmailNotification(Order order)
        {
            Console.WriteLine($"[OldProcessor][Email] Sending notification to {order.CustomerName} for order #{order.Id}.");
        }
    }

    // --- Інтерфейси для SRP-рефакторингу ---
    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order? GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    // --- Простi заглушки (реалізації) ---
    public class SimpleOrderValidator : IOrderValidator
    {
        public bool IsValid(Order order) => order != null && order.TotalAmount > 0;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, Order> _store = new();

        public void Save(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _store[order.Id] = order;
            Console.WriteLine($"[InMemoryRepo] Order #{order.Id} saved (Customer: {order.CustomerName}, Amount: {order.TotalAmount}).");
        }

        public Order? GetById(int id)
        {
            _store.TryGetValue(id, out var order);
            return order;
        }
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Console.WriteLine($"[Email] Sending confirmation to {order.CustomerName} for order #{order.Id}.");
        }
    }

    // --- OrderService (координатор, DI через конструктор) ---
    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public void ProcessOrder(Order order)
        {
            if (order == null)
            {
                Console.WriteLine("[OrderService] Order is null.");
                return;
            }

            Console.WriteLine($"[OrderService] Processing order #{order.Id} for {order.CustomerName}.");

            if (!_validator.IsValid(order))
            {
                order.Status = OrderStatus.Failed;
                Console.WriteLine($"[OrderService] Order #{order.Id} validation failed. Status set to {order.Status}.");
                return;
            }
            order.Status = OrderStatus.Validated;
            Console.WriteLine($"[OrderService] Order #{order.Id} validated. Status set to {order.Status}.");

            try
            {
                _repository.Save(order);
                order.Status = OrderStatus.Saved;
                Console.WriteLine($"[OrderService] Order #{order.Id} saved. Status set to {order.Status}.");
            }
            catch (Exception ex)
            {
                order.Status = OrderStatus.Failed;
                Console.WriteLine($"[OrderService] Order #{order.Id} failed to save: {ex.Message}. Status set to {order.Status}.");
                return;
            }

            try
            {
                _emailService.SendOrderConfirmation(order);
                order.Status = OrderStatus.Notified;
                Console.WriteLine($"[OrderService] Notification sent for order #{order.Id}. Status set to {order.Status}.");
            }
            catch (Exception ex)
            {
                order.Status = OrderStatus.Failed;
                Console.WriteLine($"[OrderService] Order #{order.Id} failed to notify: {ex.Message}. Status set to {order.Status}.");
                return;
            }

            order.Status = OrderStatus.Completed;
            Console.WriteLine($"[OrderService] Order #{order.Id} processing completed. Final status {order.Status}.");
        }
    }

    // --- Демонстрація в одному Main ---
    class Program
    {
        static void Main()
        {
            // 1) Демонстрація старого OrderProcessor
            var oldProcessor = new OrderProcessor();
            var orderA = new Order(1, "Іван Петренко", 150.50m);
            oldProcessor.ProcessOrder(orderA);

            Console.WriteLine();

            var orderB = new Order(2, "Олена Коваль", 0m);
            oldProcessor.ProcessOrder(orderB);

            Console.WriteLine("\n--- Перехід до рефакторингу (SRP) ---\n");

            // 2) Налаштування компонентів для OrderService
            IOrderValidator validator = new SimpleOrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            var orderService = new OrderService(validator, repository, emailService);

            // Валідне замовлення через OrderService
            var orderC = new Order(3, "Петро Іванов", 200m);
            orderService.ProcessOrder(orderC);

            Console.WriteLine();

            // Невалідне замовлення через OrderService
            var orderD = new Order(4, "Марія Сидоренко", -10m);
            orderService.ProcessOrder(orderD);

            Console.WriteLine();

            // Перевірка репозиторію
            var fetched = repository.GetById(3);
            Console.WriteLine(fetched != null
                ? $"[Check] Fetched order #{fetched.Id} from repository. Status: {fetched.Status}."
                : "[Check] Order #3 not found in repository.");
        }
    }
}