using System;

namespace Lab2_Patterns.Task2_AbstractFactory
{
    public class IProneFactory : IDeviceFactory
    {
        public string GetBrandName() => "IProne";
        
        public Laptop CreateLaptop()
        {
            return new IProneLaptop();
        }
        
        public Netbook CreateNetbook()
        {
            return new IProneNetbook();
        }
        
        public EBook CreateEBook()
        {
            return new IProneEBook();
        }
        
        public Smartphone CreateSmartphone()
        {
            return new IProneSmartphone();
        }
    }
    
    public class IProneLaptop : Laptop
    {
        public override void ShowInfo()
        {
            Console.WriteLine("IProne Laptop: MacBook Pro 16\", M3 Max, 32GB RAM, 1TB SSD");
        }
    }
    
    public class IProneNetbook : Netbook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("IProne Netbook: MacBook Air 13\", M2, 16GB RAM, 512GB SSD");
        }
    }
    
    public class IProneEBook : EBook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("IProne EBook: iPad Pro 12.9\", M2, 256GB, Liquid Retina XDR");
        }
    }
    
    public class IProneSmartphone : Smartphone
    {
        public override void ShowInfo()
        {
            Console.WriteLine("IProne Smartphone: iPhone 15 Pro Max, A17 Pro, 512GB");
        }
    }
}