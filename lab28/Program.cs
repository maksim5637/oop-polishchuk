using System;
using System.Text.Json;

// Класи для десеріалізації
public class CarData
{
    public string? saved_at { get; set; }
    public int total_count { get; set; }
    public List<Car>? cars { get; set; }
}

public class Car
{
    public int id { get; set; }
    public string? model { get; set; }
    public int year { get; set; }
    public int price_usd { get; set; }
    public string? body_type { get; set; }
    public string? color { get; set; }
    public int mileage_km { get; set; }
    public bool is_new { get; set; }
    public Manufacturer? manufacturer { get; set; }
    public Engine? engine { get; set; }
}

public class Manufacturer
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? country { get; set; }
    public int founded_year { get; set; }
    public string? website { get; set; }
}

public class Engine
{
    public int displacement_cc { get; set; }
    public int horsepower { get; set; }
    public int torque_nm { get; set; }
    public string? fuel_type { get; set; }
    public int cylinders { get; set; }
}

internal class Lab28Program
{
    static void Main()
    {
        string jsonData = @"{
  ""saved_at"": ""2025-05-04T10:15:00"",
  ""total_count"": 6,
  ""cars"": [
    {
      ""id"": 1,
      ""model"": ""Camry"",
      ""year"": 2023,
      ""price_usd"": 32000,
      ""body_type"": ""Sedan"",
      ""color"": ""Білий"",
      ""mileage_km"": 0,
      ""is_new"": true,
      ""manufacturer"": {
        ""id"": 1,
        ""name"": ""Toyota"",
        ""country"": ""Японія"",
        ""founded_year"": 1937,
        ""website"": ""toyota.com""
      },
      ""engine"": {
        ""displacement_cc"": 2494,
        ""horsepower"": 207,
        ""torque_nm"": 243,
        ""fuel_type"": ""Petrol"",
        ""cylinders"": 4
      }
    },
    {
      ""id"": 2,
      ""model"": ""RAV4 Hybrid"",
      ""year"": 2022,
      ""price_usd"": 38500,
      ""body_type"": ""SUV"",
      ""color"": ""Сірий"",
      ""mileage_km"": 15400,
      ""is_new"": false,
      ""manufacturer"": {
        ""id"": 1,
        ""name"": ""Toyota"",
        ""country"": ""Японія"",
        ""founded_year"": 1937,
        ""website"": ""toyota.com""
      },
      ""engine"": {
        ""displacement_cc"": 1798,
        ""horsepower"": 122,
        ""torque_nm"": 142,
        ""fuel_type"": ""Hybrid"",
        ""cylinders"": 4
      }
    },
    {
      ""id"": 3,
      ""model"": ""M5"",
      ""year"": 2024,
      ""price_usd"": 115000,
      ""body_type"": ""Sedan"",
      ""color"": ""Чорний"",
      ""mileage_km"": 3200,
      ""is_new"": true,
      ""manufacturer"": {
        ""id"": 2,
        ""name"": ""BMW"",
        ""country"": ""Німеччина"",
        ""founded_year"": 1916,
        ""website"": ""bmw.com""
      },
      ""engine"": {
        ""displacement_cc"": 2998,
        ""horsepower"": 374,
        ""torque_nm"": 500,
        ""fuel_type"": ""Diesel"",
        ""cylinders"": 6
      }
    },
    {
      ""id"": 4,
      ""model"": ""Model S"",
      ""year"": 2023,
      ""price_usd"": 89990,
      ""body_type"": ""Sedan"",
      ""color"": ""Червоний"",
      ""mileage_km"": 8750,
      ""is_new"": false,
      ""manufacturer"": {
        ""id"": 3,
        ""name"": ""Tesla"",
        ""country"": ""США"",
        ""founded_year"": 2003,
        ""website"": ""tesla.com""
      },
      ""engine"": {
        ""displacement_cc"": 0,
        ""horsepower"": 670,
        ""torque_nm"": 900,
        ""fuel_type"": ""Electric"",
        ""cylinders"": 0
      }
    },
    {
      ""id"": 5,
      ""model"": ""F-150"",
      ""year"": 2022,
      ""price_usd"": 55000,
      ""body_type"": ""Pickup"",
      ""color"": ""Синій"",
      ""mileage_km"": 42000,
      ""is_new"": false,
      ""manufacturer"": {
        ""id"": 4,
        ""name"": ""Ford"",
        ""country"": ""США"",
        ""founded_year"": 1903,
        ""website"": ""ford.com""
      },
      ""engine"": {
        ""displacement_cc"": 4951,
        ""horsepower"": 450,
        ""torque_nm"": 847,
        ""fuel_type"": ""Petrol"",
        ""cylinders"": 8
      }
    },
    {
      ""id"": 6,
      ""model"": ""Corolla"",
      ""year"": 2024,
      ""price_usd"": 26500,
      ""body_type"": ""Hatchback"",
      ""color"": ""Зелений"",
      ""mileage_km"": 0,
      ""is_new"": true,
      ""manufacturer"": {
        ""id"": 1,
        ""name"": ""Toyota"",
        ""country"": ""Японія"",
        ""founded_year"": 1937,
        ""website"": ""toyota.com""
      },
      ""engine"": {
        ""displacement_cc"": 1798,
        ""horsepower"": 122,
        ""torque_nm"": 142,
        ""fuel_type"": ""Hybrid"",
        ""cylinders"": 4
      }
    }
  ]
}";

        // Десеріалізація JSON
        var data = JsonSerializer.Deserialize<CarData>(jsonData);

        if (data != null)
        {
            Console.WriteLine($"Загальна кількість автомобілів: {data.total_count}");
            if (data.cars != null)
            {
                foreach (var car in data.cars)
                {
                    Console.WriteLine($"Модель: {car.model}, Рік: {car.year}, Ціна: {car.price_usd} USD");
                }
            }
        }
    }
}
