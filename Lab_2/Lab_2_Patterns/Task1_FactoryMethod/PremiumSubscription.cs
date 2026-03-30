using System.Collections.Generic;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class PremiumSubscription : Subscription
    {
        public PremiumSubscription()
        {
            MonthlyFee = 500;
            MinPeriod = 6;
            Channels = new List<string> 
            { 
                "HBO", "Netflix", "Megogo", "Sweet.TV", "Setanta Sports"
            };
            Features = new List<string> 
            { 
                "4K HDR", "Dolby Atmos", "Без реклами", 
                "Завантаження", "5 пристроїв"
            };
        }
    }
}