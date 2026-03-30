namespace Lab_3_StructuralPatterns.Task2_Decorator
{
    public abstract class InventoryDecorator : IHero
    {
        protected readonly IHero _hero;

        protected InventoryDecorator(IHero hero)
        {
            _hero = hero;
        }

        public virtual string Name => _hero.Name;
        public virtual int Attack => _hero.Attack;
        public virtual int Defense => _hero.Defense;

        public virtual void ShowStats()
        {
            _hero.ShowStats();
        }
    }
}