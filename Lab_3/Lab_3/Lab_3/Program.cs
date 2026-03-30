using System;
using System.Collections.Generic;
using Lab_3_StructuralPatterns.Task1_Adapter;
using Lab_3_StructuralPatterns.Task2_Decorator;
using Lab_3_StructuralPatterns.Task3_Bridge;
using Lab_3_StructuralPatterns.Task4_Proxy;
using Lab_3_StructuralPatterns.Task5_Composite;
using Lab_3_StructuralPatterns.Task6_Flyweight; 

class Program
{
    static void Main(string[] args)
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        RunTask1();
        RunTask2();
        RunTask3();
        RunTask4();
        RunTask5();
        RunTask6();
    }

    static void RunTask1()
    {
        Console.WriteLine("\nЗавдання 1: Адаптер");
        
        Logger consoleLogger = new Logger();
        Console.WriteLine("Тест консольного логера:");
        consoleLogger.Log("Систему запущено успішно.");
        consoleLogger.Warn("Виявлено незначне споживання ресурсів.");
        consoleLogger.Error("Критична помилка доступу до бази даних!");

        FileWriter writer = new FileWriter("log_lab3.txt");
        Logger fileLogger = new FileLoggerAdapter(writer);
        
        Console.WriteLine("\nТест файлового логера (запис у log_lab3.txt):");
        fileLogger.Log("Запис звичайного логу у файл.");
        fileLogger.Warn("Запис попередження у файл.");
        fileLogger.Error("Запис повідомлення про помилку у файл.");
    }

    static void RunTask2()
    {
        Console.WriteLine("\nЗавдання 2: Декоратор");

        Console.WriteLine("Warrior");
        IHero tankWarrior = new Warrior();
        tankWarrior = new ArmorDecorator(tankWarrior);
        tankWarrior = new ArmorDecorator(tankWarrior);
        tankWarrior.ShowStats();

        Console.WriteLine("\nBattle Mage");
        IHero battleMage = new Mage();
        battleMage = new WeaponDecorator(battleMage);
        battleMage = new ArtifactDecorator(battleMage);
        battleMage.ShowStats();

        Console.WriteLine("\nUltimate Paladin");
        IHero ultimatePaladin = new Paladin();
        ultimatePaladin = new ArmorDecorator(ultimatePaladin);
        ultimatePaladin = new WeaponDecorator(ultimatePaladin);
        ultimatePaladin = new ArtifactDecorator(ultimatePaladin);
        ultimatePaladin.ShowStats();
    }

    static void RunTask3()
    {
        Console.WriteLine("\nЗавдання 3: Міст");
        Shape circle = new Circle(new RasterRenderer());
        circle.Draw();

        Shape square = new Square(new VectorRenderer());
        square.Draw();
    }

    static void RunTask4()
    {
        Console.WriteLine("\nЗавдання 4: Проксі");
        string testFile = "test.txt";
        System.IO.File.WriteAllText(testFile, "Hello World\nLine 2");

        ITextReader reader = new SmartTextReader();
        reader = new SmartTextChecker(reader);
        reader = new SmartTextReaderLocker(reader, @"\.secret$");

        reader.ReadText(testFile);
        reader.ReadText("data.secret");
    }

    static void RunTask5()
    {
        Console.WriteLine("\nЗавдання 5: Компонувальник");
        var table = new LightElementNode("table", "block", "double");
        var tr = new LightElementNode("tr", "block", "double");
        
        var td1 = new LightElementNode("td", "inline", "double");
        td1.Add(new LightTextNode("Клітинка 1"));
        
        tr.Add(td1);
        table.Add(tr);

        Console.WriteLine("OuterHTML таблиці:");
        Console.WriteLine(table.OuterHTML);
    }

    static void RunTask6()
    {
        Console.WriteLine("\nЗавдання 6: Легковаговик");

        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "book.txt");

        if (!System.IO.File.Exists(filePath))
        {
            Console.WriteLine($"[Помилка]: Файл '{filePath}' не знайдено у папці з програмою.");
            return;
        }

        string[] bookLines = System.IO.File.ReadAllLines(filePath);

        long memoryBefore = GC.GetTotalMemory(true);

        var converter = new BookToHtmlConverter();
        var htmlBook = converter.Convert(bookLines);

        long memoryAfter = GC.GetTotalMemory(true);

        Console.WriteLine("Результат HTML-верстки вашого тексту:");
        Console.WriteLine(htmlBook.InnerHTML);

        Console.WriteLine("\nЗвіт по пам'яті (Flyweight):");
        Console.WriteLine($"Оброблено рядків: {bookLines.Length}");
        Console.WriteLine($"Використано пам'яті: {(memoryAfter - memoryBefore) / 1024.0:F2} KB");
    }
}