using System;

namespace Lab4_PR1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PR1: Завдання 1, 2, 5\n");

            Task1.SupportMenu.Run();

            Task2.MediatorDemo.Run();

            Task5.MementoDemo.Run();

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}