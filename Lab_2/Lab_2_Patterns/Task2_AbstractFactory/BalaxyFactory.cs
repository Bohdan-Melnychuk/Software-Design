using System;

namespace Lab2_Patterns.Task2_AbstractFactory
{
    public class BalaxyFactory : IDeviceFactory
    {
        public string GetBrandName() => "Balaxy";
        
        public Laptop CreateLaptop()
        {
            return new BalaxyLaptop();
        }
        
        public Netbook CreateNetbook()
        {
            return new BalaxyNetbook();
        }
        
        public EBook CreateEBook()
        {
            return new BalaxyEBook();
        }
        
        public Smartphone CreateSmartphone()
        {
            return new BalaxySmartphone();
        }
    }
    
    public class BalaxyLaptop : Laptop
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Balaxy Laptop: Balaxy Book 15\", Intel i7, 16GB RAM, 512GB SSD");
        }
    }
    
    public class BalaxyNetbook : Netbook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Balaxy Netbook: Balaxy Go 11.6\", Intel N100, 8GB RAM, 256GB SSD");
        }
    }
    
    public class BalaxyEBook : EBook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Balaxy EBook: Balaxy Tab 8\", E-Ink, 16GB, Backlight");
        }
    }
    
    public class BalaxySmartphone : Smartphone
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Balaxy Smartphone: Balaxy S24 Ultra, Exynos 2400, 12GB RAM, 512GB");
        }
    }
}