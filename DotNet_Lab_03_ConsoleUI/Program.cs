using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using DotNet_Lab01_Core;


class Program
{
    static void Main()
    {
        // Завдання №2
        Course course = new Course(
            "C#",
            5,
            DateTime.Now,
            DateTime.Now.AddMonths(6)
        );

        Console.WriteLine(
            $"Course duration: {course.GetCourseDuration()} days\n"
        );

        // Завдання №3
        CourseManager manager = new CourseManager();
        manager.AddCourse(
            new Course(
                "C#",
                5,
                DateTime.Now,
                DateTime.Now.AddMonths(4)
            )
        );

        manager.AddCourse(
            new Course(
                "Algorithms",
                4,
                DateTime.Now,
                DateTime.Now.AddMonths(3)
            )
        );

        manager.AddCourse(
            new Course(
                "Data Structures",
                2,
                DateTime.Now,
                DateTime.Now.AddMonths(5)
            )
        );
        // Завдання №4
        foreach (var courses in manager)
        {
            Console.WriteLine(courses);
        }
        // Завдання №5
        Course? foundCourse = manager.FindById(2);

        if(foundCourse != null)
        {
            Console.WriteLine("Found course:");
            Console.WriteLine(foundCourse);
        }
        else
        {
            Console.WriteLine("Course not found.");
        }


        Console.WriteLine("UncompletedCourses:\n");
        foreach (var courses in manager.GetUncompletedCourses())
        {
            Console.WriteLine(courses);
        }

        // Завдання №6
        var tags = new HashSet<string> {"C#", "Algorithms", "Data Structures", "OOP"};
        tags.Add("C#");
        Console.WriteLine("\nTags:");
        foreach(var tag in tags)
        {
            Console.WriteLine(tag);
        }
        // Копію не додано


        var tags_2 = new HashSet<string> {"English","Algorithms","UI/UX Design","Operating Systems","OOP"};
        var union = new HashSet<string>(tags);
        union.UnionWith(tags_2);
        Console.WriteLine("\nUnion:");
        foreach(var tag in union)
        {
            Console.WriteLine(tag);
        }

        var intersect = new HashSet<string>(tags);
		intersect.IntersectWith(tags_2);
        Console.WriteLine("\nIntersect:");
        foreach(var tag in intersect)
        {
            Console.WriteLine(tag);
        }

    }
}