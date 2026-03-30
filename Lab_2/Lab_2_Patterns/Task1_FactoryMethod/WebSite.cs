using System;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class WebSite : SubscriptionCreator
    {
        protected override string GetCreatorName() => "Веб-сайт";
        
        public override Subscription CreateSubscription(string type)
        {
            Console.WriteLine("Оформлення підписки через веб-сайт...");
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