using System;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public abstract class SubscriptionCreator
    {
        public abstract Subscription CreateSubscription(string type);
        
        public void PurchaseSubscription(string type)
        {
            Subscription subscription = CreateSubscription(type);
            
            Console.WriteLine($"\nПідписку придбано через {GetCreatorName()}");
            subscription.ShowDetails();
        }
        
        protected abstract string GetCreatorName();
    }
}