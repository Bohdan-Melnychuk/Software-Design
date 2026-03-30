using System;
using System.Collections.Generic;

namespace Lab2_Patterns.Task1_FactoryMethod
{
    public abstract class Subscription
    {
        public decimal MonthlyFee { get; protected set; }
        public int MinPeriod { get; protected set; }
        public List<string> Channels { get; protected set; }
        public List<string> Features { get; protected set; }
        
        public void ApplyDiscount(decimal discountPercentage)
        {
            if (discountPercentage > 0 && discountPercentage < 1)
            {
                MonthlyFee *= discountPercentage;
            }
        }
        
        public virtual void ShowDetails()
        {
            Console.WriteLine($"Тип: {GetType().Name}");
            Console.WriteLine($"Щомісячна плата: {MonthlyFee} грн");
            Console.WriteLine($"Мінімальний період: {MinPeriod} міс.");
            
            Console.WriteLine("Канали:");
                
            Console.WriteLine("Можливості:");
        }
    }
}