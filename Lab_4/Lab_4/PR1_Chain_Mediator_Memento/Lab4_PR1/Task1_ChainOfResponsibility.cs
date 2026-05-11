using System;

namespace Lab4_PR1.Task1
{
    public abstract class SupportHandler
    {
        protected SupportHandler _next;

        public void SetNext(SupportHandler next)
        {
            _next = next;
        }

        public void Handle(string problem, int level)
        {
            if (CanHandle(problem, level))
            {
                HandleInternal(problem);
            }
            else if (_next != null)
            {
                _next.Handle(problem, level);
            }
            else
            {
                Console.WriteLine("Жоден рівень не підійшов. Почнемо спочатку.");
            }
        }

        protected abstract bool CanHandle(string problem, int level);
        protected abstract void HandleInternal(string problem);
    }

    public class Level1Handler : SupportHandler
    {
        protected override bool CanHandle(string problem, int level)
        {
            return level == 1;
        }

        protected override void HandleInternal(string problem)
        {
            Console.WriteLine($"Рівень 1 вирішив: {problem}");
        }
    }

    public class Level2Handler : SupportHandler
    {
        protected override bool CanHandle(string problem, int level)
        {
            return level == 2;
        }

        protected override void HandleInternal(string problem)
        {
            Console.WriteLine($"Рівень 2 вирішив: {problem}");
        }
    }

    public class Level3Handler : SupportHandler
    {
        protected override bool CanHandle(string problem, int level)
        {
            return level == 3;
        }

        protected override void HandleInternal(string problem)
        {
            Console.WriteLine($"Рівень 3 вирішив: {problem}");
        }
    }

    public class Level4Handler : SupportHandler
    {
        protected override bool CanHandle(string problem, int level)
        {
            return level == 4;
        }

        protected override void HandleInternal(string problem)
        {
            Console.WriteLine($"Рівень 4 вирішив: {problem}");
        }
    }

    public static class SupportMenu
    {
        public static void Run()
        {
            Console.WriteLine("\nЗавдання 1: Ланцюжок відповідальностей\n");

            var level1 = new Level1Handler();
            var level2 = new Level2Handler();
            var level3 = new Level3Handler();
            var level4 = new Level4Handler();

            level1.SetNext(level2);
            level2.SetNext(level3);
            level3.SetNext(level4);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nСистема підтримки");
                Console.WriteLine("Оберіть рівень проблеми (1-4) або 0 для виходу:");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Некоректне введення. Спробуйте ще раз.");
                    continue;
                }

                if (choice == 0)
                {
                    exit = true;
                    continue;
                }

                if (choice < 1 || choice > 4)
                {
                    Console.WriteLine("Невірний рівень. Виберіть 1-4.");
                    continue;
                }

                Console.WriteLine("Опишіть вашу проблему:");
                string problem = Console.ReadLine();

                level1.Handle(problem, choice);
            }
        }
    }
}