namespace Lab_3_StructuralPatterns.Task2_Decorator
{
    public interface IHero
    {
        string Name { get; }
        int Attack { get; }
        int Defense { get; }
        void ShowStats();
    }
}