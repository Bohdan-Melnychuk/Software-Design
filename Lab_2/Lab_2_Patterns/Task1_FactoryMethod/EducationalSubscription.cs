using System.Collections.Generic;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class EducationalSubscription : Subscription
    {
        public EducationalSubscription()
        {
            MonthlyFee = 200;
            MinPeriod = 3;
            Channels = new List<string> 
            { 
                "Discovery", "National Geographic", "History Channel", 
                "Animal Planet", "BBC Earth"
            };
            Features = new List<string> 
            { 
                "4K якість", "Навчальні курси", "Документальні фільми"
            };
        }
    }
}