using System;
using System.Collections.Generic;
using System.Linq;

public class InvalidLessonException : Exception
{
    public InvalidLessonException(string message) : base(message) { }
}

public class TimeOverlapException : Exception
{
    public TimeOverlapException(string message) : base(message) { }
}

public class Lesson
{
    public string Subject { get; set; }
    public string Teacher { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }

    public Lesson(string subject, string teacher, DayOfWeek day, TimeSpan start, int duration)
    {
        if (duration <= 0)
            throw new InvalidLessonException("Тривалість заняття має бути більше 0 хвилин.");

        Subject = subject;
        Teacher = teacher;
        Day = day;
        StartTime = start;
        DurationMinutes = duration;
    }

    public TimeSpan EndTime => StartTime.Add(TimeSpan.FromMinutes(DurationMinutes));

    public override string ToString() =>
        $"{Day}: {Subject} ({Teacher}) {StartTime}-{EndTime}";
}

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);
    public IEnumerable<T> GetAll() => items;
    public T? Find(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
}

public class Schedule
{
    private Repository<Lesson> lessons = new Repository<Lesson>();

    public void AddLesson(Lesson lesson)
    {
        foreach (var l in lessons.GetAll())
        {
            if (l.Day == lesson.Day &&
                lesson.StartTime < l.EndTime &&
                lesson.EndTime > l.StartTime)
            {
                throw new TimeOverlapException($"Перетин занять: {l.Subject} та {lesson.Subject}");
            }
        }
        lessons.Add(lesson);
    }

    public IEnumerable<Lesson> GetLessons() => lessons.GetAll();

    public int TotalHours() =>
        lessons.GetAll().Sum(l => l.DurationMinutes) / 60;

    public double TeacherLoad(string teacher)
    {
        int total = lessons.GetAll().Sum(l => l.DurationMinutes);
        int teacherMinutes = lessons.GetAll()
            .Where(l => l.Teacher == teacher)
            .Sum(l => l.DurationMinutes);

        return total == 0 ? 0 : (double)teacherMinutes / total * 100;
    }
}

class Program
{
    static void Main()
    {
        try
        {
            Schedule schedule = new Schedule();

            Lesson math = new Lesson("Математика", "Іваненко", DayOfWeek.Monday, new TimeSpan(9, 0, 0), 90);
            Lesson physics = new Lesson("Фізика", "Петренко", DayOfWeek.Monday, new TimeSpan(10, 0, 0), 60);

            schedule.AddLesson(math);
            schedule.AddLesson(physics); 

            foreach (var l in schedule.GetLessons())
                Console.WriteLine(l);

            Console.WriteLine($"Загальна кількість годин: {schedule.TotalHours()}");
            Console.WriteLine($"Завантаженість Іваненка: {schedule.TeacherLoad("Іваненко")}%");
        }
        catch (InvalidLessonException ex)
        {
            Console.WriteLine("Помилка: " + ex.Message);
        }
        catch (TimeOverlapException ex)
        {
            Console.WriteLine("Помилка: " + ex.Message);
        }
    }
}
