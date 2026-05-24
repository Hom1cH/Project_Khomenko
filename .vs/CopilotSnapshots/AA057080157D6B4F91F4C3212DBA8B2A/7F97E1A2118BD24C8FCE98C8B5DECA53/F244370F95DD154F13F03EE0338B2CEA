using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DotNet_Lab01_Core;

class Program
{
    static void Main()
    {
        ParacTask task_2 = new ParacTask
        {
            TaskName = "Lab 2",
            TaskDescription ="Дослідити роботу структур",

            Progress = new TaskProgress
            {
                Percent = 20,
                IsCompleted = false
            }
        };

        Console.WriteLine($"Before: {task_2.Progress.Percent}%");

        task_2.ModifyProgress(task_2.Progress);

        Console.WriteLine($"After: {task_2.Progress.Percent}%\n");


        // BOXING
        int number = 42;
        object boxedNumber = number;

        Console.WriteLine($"Boxed value: {boxedNumber}");

        // UNBOXING
        int unboxedNumber = (int)boxedNumber;

        Console.WriteLine($"Unboxed value: {unboxedNumber}");

        const int iterations = 1_000_000;

        // ArrayList
        ArrayList arrayList = new ArrayList();

        Stopwatch swArrayList = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            arrayList.Add(i);
        }

        swArrayList.Stop();

        // List<int>
        List<int> list = new List<int>();

        Stopwatch swList = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            list.Add(i);
        }

        swList.Stop();

        Console.WriteLine();
        Console.WriteLine("Performance comparison:");
        Console.WriteLine($"ArrayList: {swArrayList.ElapsedMilliseconds} ms");
        Console.WriteLine($"List<int>: {swList.ElapsedMilliseconds} ms");

        Console.WriteLine();
        Console.WriteLine($"Difference: {swArrayList.ElapsedMilliseconds - swList.ElapsedMilliseconds} ms\n");

        // LINQ List<T>
        List<Course> courses = new List<Course>
        {
            new Course("C# Programming",5,new DateTime(2026, 2, 1),new DateTime(2026, 6, 1)),
            new Course("Algorithms",4,new DateTime(2026, 2, 3),new DateTime(2026, 6, 5)),
            new Course("Databases",3,new DateTime(2026, 2, 5),new DateTime(2026, 6, 10)),
            new Course("Computer Networks",4,new DateTime(2026, 2, 7),new DateTime(2026, 6, 12)),
            new Course("Operating Systems",5,new DateTime(2026, 2, 10),new DateTime(2026, 6, 15)),
            new Course("Physics",2,new DateTime(2026, 2, 11),new DateTime(2026, 5, 30)),
            new Course("Linear Algebra",3,new DateTime(2026, 2, 12),new DateTime(2026, 6, 8)),
            new Course("UI/UX Design",2,new DateTime(2026, 2, 15),new DateTime(2026, 5, 25)),
            new Course("Software Engineering",5,new DateTime(2026, 2, 17),new DateTime(2026, 6, 20)),
            new Course("English",1,new DateTime(2026, 2, 20),new DateTime(2026, 5, 28))
        };

        // LINQ (Where)
        List<Course> difficultCourses = courses
        .Where(course => course.Credits > 3)
        .ToList();

        Console.WriteLine("Courses with credits > 3:");

        foreach (var difficultCourse in difficultCourses)
        {
            Console.WriteLine(
                $"{difficultCourse.CourseName} | Credits: {difficultCourse.Credits}"
            );
        }

        // LINQ (OrderBy, ThenBy)
        List<Course> sortedCourses = courses
        .OrderBy(course => course.Credits)
        .ThenBy(course => course.CourseName)
        .ToList();

        Console.WriteLine("\nSorted courses:");

        foreach (var sortedCourse in sortedCourses)
        {
            Console.WriteLine(
                $"{sortedCourse.CourseName} | Credits: {sortedCourse.Credits}"
            );
        }

        // LINQ (Select)
        List<string> courseNames = courses
        .Select(course => course.CourseName)
        .ToList();

        Console.WriteLine("\nCourse names:");

        foreach (var name in courseNames)
        {
            Console.WriteLine(name);
        }

        // LINQ (FirstOrDefault)    
        Course? foundCourse = courses
        .FirstOrDefault(course => course.CourseName == "OOP");


        if (foundCourse != null)
        {
            Console.WriteLine("\nCourse found:");
            Console.WriteLine(
                $"{foundCourse.CourseName} | Credits: {foundCourse.Credits}"
            );
        }
        else
        {
            Console.WriteLine("\nCourse not found.");
        }
    }
}
        
