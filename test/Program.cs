using DotNet_Lab01_Core;

class Program
{
    static void Main()
    {
        Console.WriteLine("----------------------------------------------------------");

        List<IShowable> items = new List<IShowable>();

        items.Add(new Course("C# Basics", 5, DateTime.Now.AddMonths(3)));
        items.Add(new ParacTask("Lab 1", DateTime.Now.AddDays(7), 80, 3, "Create console app"));

        Console.WriteLine("\nPolymorphism :\n");

        foreach (IShowable item in items)
        {
            item.ShowInfo(); 
        }
    }
}