using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3Namespace
{
    public class Movie
    {
        public string Title { get; set; }
        public double Rating { get; set; }

        public Movie(string title, double rating)
        {
            Title = title;
            Rating = rating;
        }

        public virtual void ShowInfo()
        {
            Console.WriteLine($"Фільм: {Title}, Рейтинг: {Rating}");
        }
    }

    public class ActionMovie : Movie
    {
        public string StuntCoordinator { get; set; }

        public ActionMovie(string title, double rating, string stuntCoordinator)
            : base(title, rating)
        {
            StuntCoordinator = stuntCoordinator;
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"[Бойовик] {Title}, Рейтинг: {Rating}, Каскадер: {StuntCoordinator}");
        }
    }

    public class ComedyMovie : Movie
    {
        public string Comedian { get; set; }

        public ComedyMovie(string title, double rating, string comedian)
            : base(title, rating)
        {
            Comedian = comedian;
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"[Комедія] {Title}, Рейтинг: {Rating}, Комік: {Comedian}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Movie> movies = new List<Movie>
            {
                new ActionMovie("Mad Max", 8.5, "John Smith"),
                new ComedyMovie("The Mask", 7.9, "Jim Carrey"),
                new ActionMovie("Die Hard", 9.0, "Mike Johnson"),
                new ComedyMovie("Home Alone", 8.2, "Macaulay Culkin")
            };

            Console.WriteLine("=== Список фільмів ===");
            foreach (var movie in movies)
                movie.ShowInfo(); 

            double avgAction = movies.OfType<ActionMovie>().Average(m => m.Rating);
            double avgComedy = movies.OfType<ComedyMovie>().Average(m => m.Rating);

            Console.WriteLine($"\nСередній рейтинг бойовиків: {avgAction:F2}");
            Console.WriteLine($"Середній рейтинг комедій: {avgComedy:F2}");

            var topMovie = movies.OrderByDescending(m => m.Rating).First();
            Console.WriteLine($"\nТоп-1 фільм: {topMovie.Title} з рейтингом {topMovie.Rating}");
        }
    }
}