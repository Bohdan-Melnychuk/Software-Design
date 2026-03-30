using System;

namespace Lab_3_StructuralPatterns.Task2_Decorator
{
    public class Warrior : IHero
    {
        public string Name => "Warrior";
        public int Attack => 15;
        public int Defense => 10;

        public void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense}");
        }
    }

    public class Mage : IHero
    {
        public string Name => "Mage";
        public int Attack => 25;
        public int Defense => 5;

        public void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense}");
        }
    }

    public class Paladin : IHero
    {
        public string Name => "Paladin";
        public int Attack => 12;
        public int Defense => 15;

        public void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense}");
        }
    }
}