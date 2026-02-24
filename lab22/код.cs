using System;
using System.Collections.Generic;

namespace Lab22
{
    public interface IWorker
    {
        string Name { get; }
        void DoWork();
    }

    public interface ISalariedWorker : IWorker
    {
        decimal CalculateSalary();
    }

    public class Employee : ISalariedWorker
    {
        public string Name { get; private set; }

        public Employee(string name)
        {
            Name = name;
        }

        public void DoWork()
        {
            Console.WriteLine($"{Name} is working as employee.");
        }

        public decimal CalculateSalary()
        {
            return 10000m;
        }
    }

    public class Volunteer : IWorker
    {
        public string Name { get; private set; }

        public Volunteer(string name)
        {
            Name = name;
        }

        public void DoWork()
        {
            Console.WriteLine($"{Name} is volunteering.");
        }
    }

    class Program
    {
        static void PrintSalary(ISalariedWorker worker)
        {
            Console.WriteLine($"{worker.Name} salary: {worker.CalculateSalary()}");
        }

        static void Main()
        {
            List<IWorker> team = new List<IWorker>
            {
                new Employee("Petro"),
                new Volunteer("Olena")
            };

            foreach (var w in team)
            {
                w.DoWork();
            }

            Console.WriteLine("\n=== Salaries ===");
            var salaried = new Employee("Ivan");
            PrintSalary(salaried);

        }
    }
}