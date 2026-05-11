using System;

namespace Lab4_PR3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Завдання 4 (Strategy + Image)\n");

            IImageLoadStrategy fileStrategy = new FileSystemLoadStrategy();
            IImageLoadStrategy networkStrategy = new NetworkLoadStrategy();

            Console.WriteLine("Зображення з файлової системи");
            string fileResult = fileStrategy.Load("photo.jpg");
            Console.WriteLine($"Результат завантаження: {fileResult}");
            
            var localImage = new ImageNode("photo.jpg", fileStrategy);
            Console.WriteLine($"Згенерований HTML: {localImage.OuterHTML}\n");

            Console.WriteLine("Зображення з мережі");
            string networkResult = networkStrategy.Load("https://example.com/image.png");
            Console.WriteLine($"Результат завантаження: {networkResult}");
            
            var webImage = new ImageNode("https://example.com/image.png", networkStrategy);
            Console.WriteLine($"Згенерований HTML: {webImage.OuterHTML}\n");

            Console.WriteLine("Зміна стратегії (файл -> мережа)");
            Console.WriteLine("До зміни: файлова стратегія");
            Console.WriteLine($"Результат: {fileStrategy.Load("photo.jpg")}");
            
            Console.WriteLine("Після зміни: мережева стратегія");
            string newResult = networkStrategy.Load("photo.jpg");
            Console.WriteLine($"Результат: {newResult}");
            
            Console.WriteLine("\nВисновок: Стратегію можна змінювати динамічно через SetStrategy()");
            Console.WriteLine("Залежно від href обирається потрібний спосіб завантаження.");

            Console.WriteLine("\nНатисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}