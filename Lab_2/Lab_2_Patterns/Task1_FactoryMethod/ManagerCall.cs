using System;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class ManagerCall : SubscriptionCreator
    {
        protected override string GetCreatorName() => "Дзвінок менеджеру";
        
        public override Subscription CreateSubscription(string type)
        {
            Console.WriteLine("Оформлення підписки через менеджера...");
            Console.WriteLine("Менеджер консультує вас...");
            
            return type.ToLower() switch
            {
                "domestic" => new DomesticSubscription(),
                "educational" => new EducationalSubscription(),
                "premium" => new PremiumSubscription(),
                _ => throw new ArgumentException("Невідомий тип підписки")
            };
        }
    }
}