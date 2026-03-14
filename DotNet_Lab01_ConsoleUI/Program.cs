using System;
using System.Collections.Generic;
using DotNet_Lab01_Core;

class Program
{
    static void Main()
    {
        var course = new Course(".NET Programming", 100, new DateTime(2026, 3, 3), new DateTime(2026, 6, 30));
        var task = new ParacTask("Implement a console application", new DateTime(2026, 4, 15), 10, 8, "Create a console application.");
        var reminder = new Reminder("Submit the project report", new DateTime(2026, 4, 20, 14, 0, 0), 1);
        Console.WriteLine(course.ToString());
        Console.WriteLine(task.ToString());
        Console.WriteLine(reminder.ToString());
        
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"Memory used: {GC.GetTotalMemory(false) / 1024 } KB");
    }
}