using System;

namespace Lab2_Patterns.Task2_AbstractFactory
{
    public class KiaomiFactory : IDeviceFactory
    {
        public string GetBrandName() => "Kiaomi";
        
        public Laptop CreateLaptop()
        {
            return new KiaomiLaptop();
        }
        
        public Netbook CreateNetbook()
        {
            return new KiaomiNetbook();
        }
        
        public EBook CreateEBook()
        {
            return new KiaomiEBook();
        }
        
        public Smartphone CreateSmartphone()
        {
            return new KiaomiSmartphone();
        }
    }
    
    public class KiaomiLaptop : Laptop
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Kiaomi Laptop: Kiaomi Book Pro 15.6\", Ryzen 7, 16GB RAM, 512GB SSD");
        }
    }
    
    public class KiaomiNetbook : Netbook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Kiaomi Netbook: Kiaomi Air 12.5\", Celeron, 4GB RAM, 128GB SSD");
        }
    }
    
    public class KiaomiEBook : EBook
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Kiaomi EBook: Kiaomi Reader 6\", E-Ink, 8GB, WiFi");
        }
    }
    
    public class KiaomiSmartphone : Smartphone
    {
        public override void ShowInfo()
        {
            Console.WriteLine("Kiaomi Smartphone: Kiaomi 14 Ultra, Snapdragon 8 Gen 3, 12GB RAM, 256GB");
        }
    }
}