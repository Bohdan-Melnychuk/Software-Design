using System;

namespace Lab_3_StructuralPatterns.Task2_Decorator
{
    public class ArmorDecorator : InventoryDecorator
    {
        public ArmorDecorator(IHero hero) : base(hero) { }

        public override int Defense => _hero.Defense + 10;
        public override string Name => _hero.Name + " in Armor";

        public override void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense} (Bonus: +10 Def)");
        }
    }

    public class WeaponDecorator : InventoryDecorator
    {
        public WeaponDecorator(IHero hero) : base(hero) { }

        public override int Attack => _hero.Attack + 7;
        public override string Name => _hero.Name + " with Sword";

        public override void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense} (Bonus: +7 Atk)");
        }
    }

    public class ArtifactDecorator : InventoryDecorator
    {
        public ArtifactDecorator(IHero hero) : base(hero) { }

        public override int Attack => _hero.Attack + 5;
        public override int Defense => _hero.Defense + 5;
        public override string Name => _hero.Name + " with Artifact";

        public override void ShowStats()
        {
            Console.WriteLine($"{Name}: Attack={Attack}, Defense={Defense} (Bonus: +5 Atk, +5 Def)");
        }
    }
}