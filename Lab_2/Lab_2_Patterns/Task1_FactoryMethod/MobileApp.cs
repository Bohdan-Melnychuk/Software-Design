using System;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class MobileApp : SubscriptionCreator
    {
        protected override string GetCreatorName() => "Мобільний додаток";
        
        public override Subscription CreateSubscription(string type)
        {
            Console.WriteLine("Оформлення підписки через мобільний додаток...");
            Console.WriteLine("Застосовано знижку 10% за перший місяць!");
            
            var subscription = type.ToLower() switch
            {
                "domestic" => (Subscription)new DomesticSubscription(),
                "educational" => (Subscription)new EducationalSubscription(),
                "premium" => (Subscription)new PremiumSubscription(),
                _ => throw new ArgumentException("Невідомий тип підписки")
            };
            
            subscription.ApplyDiscount(0.9m);  
            
            return subscription;
        }
    }
}