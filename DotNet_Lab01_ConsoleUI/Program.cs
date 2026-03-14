using System;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"Memory used: {GC.GetTotalMemory(false) / 1024 } KB");
    }
}