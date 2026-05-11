using System;

namespace Lab4_PR2
{
    public static class ObserverDemo
    {
        public static void Run()
        {
            Console.WriteLine("Завдання 3: Спостерігач\n");

            LightElementNode div = new LightElementNode("div", "block", "double");

            div.AddEventListener("click", (sender, args) =>
            {
                Console.WriteLine($"Обробник 1: {args.Data}");
            });

            div.AddEventListener("click", (sender, args) =>
            {
                Console.WriteLine($"Обробник 2: {args.Data}");
            });

            div.AddEventListener("mouseover", (sender, args) =>
            {
                Console.WriteLine($"Mouseover: {args.Data}");
            });

            Console.WriteLine("Симуляція події 'click'");
            div.SimulateEvent("click");

            Console.WriteLine("\nСимуляція події 'mouseover'");
            div.SimulateEvent("mouseover");

            Console.WriteLine("\nСимуляція події 'dblclick'");
            div.SimulateEvent("dblclick");
        }
    }
}