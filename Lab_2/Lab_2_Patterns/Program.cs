using System;
using System.Threading.Tasks;
using System.Threading;
using Lab2_Patterns.Task1_FactoryMethod;
using Lab2_Patterns.Task2_AbstractFactory;
using Lab2_Patterns.Task3_Singleton;
using Lab2_Patterns.Task4_Prototype;
using Lab2_Patterns.Task5_Builder;

namespace Lab2_Patterns
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("ЛАБОРАТОРНА РОБОТА №2");
            
            Task1_Demo();
            
            Task2_Demo();
            
            Task3_Demo();
            
            Task4_Demo();
            
            Task5_Demo();
            
            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
        
        static void Task1_Demo()
        {
            Console.WriteLine("\nЗАВДАННЯ 1: Фабричний метод");
            
            SubscriptionCreator website = new WebSite();
            SubscriptionCreator mobileApp = new MobileApp();
            SubscriptionCreator managerCall = new ManagerCall();
            
            website.PurchaseSubscription("domestic");
            mobileApp.PurchaseSubscription("educational");
            managerCall.PurchaseSubscription("premium");
        }
        
        static void Task2_Demo()
        {
            Console.WriteLine("\nЗАВДАННЯ 2: Абстрактна фабрика");
            
            IDeviceFactory[] factories = new IDeviceFactory[]
            {
                new IProneFactory(),
                new KiaomiFactory(),
                new BalaxyFactory()
            };
            
            foreach (var factory in factories)
            {
                Console.WriteLine($"\nВиробництво техніки {factory.GetBrandName()}");
                
                Laptop laptop = factory.CreateLaptop();
                Netbook netbook = factory.CreateNetbook();
                EBook ebook = factory.CreateEBook();
                Smartphone smartphone = factory.CreateSmartphone();
                
                laptop.ShowInfo();
                netbook.ShowInfo();
                ebook.ShowInfo();
                smartphone.ShowInfo();
            }
        }
        
        static void Task3_Demo()
        {
            Console.WriteLine("\nЗАВДАННЯ 3: Одинак");
            
            Console.WriteLine("Запускаємо декілька потоків для отримання екземпляра Authenticator...\n");
            
            const int threadCount = 5;
            Thread[] threads = new Thread[threadCount];
            Guid[] instanceIds = new Guid[threadCount];
            
            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;
                threads[i] = new Thread(() =>
                {
                    Thread.Sleep(new Random().Next(10, 50));
                    
                    Authenticator auth = Authenticator.GetInstance();
                    instanceIds[threadIndex] = auth.InstanceId;
                    
                    auth.Authenticate($"user{threadIndex}", "password123");
                });
                
                threads[i].Start();
            }
            
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            Console.WriteLine("\nПеревіряємо, чи всі потоки отримали той самий екземпляр:");
            for (int i = 1; i < threadCount; i++)
            {
                Console.WriteLine($"Потік 0 та Потік {i}: " +
                    (instanceIds[0] == instanceIds[i] ? "ОДНАКОВІ" : "РІЗНІ"));
            }
        }
        
        static void Task4_Demo()
        {
            Console.WriteLine("\nЗАВДАННЯ 4: Прототип");
            
            Console.WriteLine("Створюємо оригінальне сімейство вірусів:\n");
            
            Virus grandParent = new Virus(5.2, 30, "Ковід-Пращур", "Coronaviridae");
            
            Virus parent1 = new Virus(3.1, 15, "Ковід-Альфа", "Coronaviridae");
            Virus parent2 = new Virus(3.3, 14, "Ковід-Бета", "Coronaviridae");
            
            grandParent.AddChild(parent1);
            grandParent.AddChild(parent2);
            
            Virus child1 = new Virus(1.2, 5, "Ковід-Гамма", "Coronaviridae");
            Virus child2 = new Virus(1.1, 4, "Ковід-Дельта", "Coronaviridae");
            Virus child3 = new Virus(1.3, 3, "Ковід-Омікрон", "Coronaviridae");
            
            parent1.AddChild(child1);
            parent1.AddChild(child2);
            parent2.AddChild(child3);
            
            Console.WriteLine("ОРИГІНАЛ:");
            grandParent.DisplayInfo();
            
            Console.WriteLine("\nКлонуємо вірус-прадіда");
            Virus clonedGrandParent = (Virus)grandParent.Clone();
            clonedGrandParent.Name = "Ковід-Клон (клон)";
            
            Console.WriteLine("\nКЛОН:");
            clonedGrandParent.DisplayInfo();
            
            Console.WriteLine("\nМодифікуємо оригінал");
            grandParent.Children[0].Children[0].Name = "Ковід-Мутований";
            Console.WriteLine("Після модифікації оригіналу, клон залишився незмінним:");
            
            Console.WriteLine("\nОРИГІНАЛ (модифікований):");
            grandParent.DisplayInfo();
            
            Console.WriteLine("\nКЛОН (незмінний):");
            clonedGrandParent.DisplayInfo();
        }
        
        static void Task5_Demo()
        {
            Console.WriteLine("\nЗАВДАННЯ 5: Будівельник");
            
            Console.WriteLine("Створюємо ГЕРОЯ:");
            var heroBuilder = new HeroBuilder();
            var director = new CharacterDirector(heroBuilder);
            Character hero = director.CreateDefaultHero();
            hero.ShowInfo();
            
            Console.WriteLine("\nСтворюємо ВОРОГА:");
            var enemyBuilder = new EnemyBuilder();
            director.SetBuilder(enemyBuilder);
            Character enemy = director.CreateDefaultEnemy();
            enemy.ShowInfo();
            
            Console.WriteLine("\nСтворюємо КАСТОМНОГО персонажа:");
            var customBuilder = new HeroBuilder();
            Character custom = customBuilder
                .SetName("Майстер Лі")
                .SetHeight("170 см")
                .SetBuild("Струнка")
                .SetHairColor("Чорний")
                .SetEyeColor("Карі")
                .SetClothing("Кімоно майстра")
                .AddInventoryItem("Посох дракона")
                .AddInventoryItem("Свиток мудрості")
                .AddInventoryItem("Чарівний амулет")
                .AddGoodDeed("Навчив 100 учнів")
                .AddGoodDeed("Переміг 10 демонів")
                .Build();
            custom.ShowInfo();
        }
    }
}