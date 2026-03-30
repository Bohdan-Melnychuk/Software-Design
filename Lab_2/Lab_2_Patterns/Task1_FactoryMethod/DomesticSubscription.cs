using System.Collections.Generic;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public class DomesticSubscription : Subscription
    {
        public DomesticSubscription()
        {
            MonthlyFee = 150;
            MinPeriod = 1;
            Channels = new List<string> 
            { 
                "1+1", "Інтер", "СТБ", "Новий канал", "ICTV"
            };
            Features = new List<string> 
            { 
                "HD якість", "Архів 7 днів", "2 пристрої"
            };
        }
    }
}