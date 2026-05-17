using System;
using System.Collections.Generic;
using Lab_3_StructuralPatterns.Task1_Adapter;
using Lab_3_StructuralPatterns.Task2_Decorator;
using Lab_3_StructuralPatterns.Task3_Bridge;
using Lab_3_StructuralPatterns.Task4_Proxy;
using Lab_3_StructuralPatterns.Task5_Composite;
using Lab_3_StructuralPatterns.Task6_Flyweight;
using Lab_3_StructuralPatterns.MKR_1_TemplateMethod;
using Lab_3_StructuralPatterns.MKR_2_Iterator;
using Lab_3_StructuralPatterns.MKR_3_Command;
using Lab_3_StructuralPatterns.MKR_4_State;
using Lab_3_StructuralPatterns.MKR_5_Visitor;

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
        RunTask7();
        RunTask8();
        RunTask9();
        RunTask10();
        RunTask11();
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

    static void RunTask7()
    {
        Console.WriteLine("\nMKR_1 Шаблонний метод (Lifecycle Hooks)");

        var div = new LoggedLightElement("div");
        div.ApplyStyles(new[] { "container", "main" });

        var p = new LoggedLightElement("p", "inline");
        p.Add(new LightTextNode("Привіт зі шаблонного методу!"));

        div.Add(p);

        Console.WriteLine("\nРезультуючий HTML:");
        Console.WriteLine(div.OuterHTML);
    }

    static void RunTask8()
    {
        Console.WriteLine("\nMKR_2 Ітератор (DFS / BFS)");

        var root = new IterableLightElement("div");
        var ul = new IterableLightElement("ul");
        var li1 = new IterableLightElement("li");
        var li2 = new IterableLightElement("li");

        li1.Add(new LightTextNode("Елемент 1"));
        li2.Add(new LightTextNode("Елемент 2"));
        ul.Add(li1);
        ul.Add(li2);
        root.Add(ul);
        root.Add(new LightTextNode("Текст у root"));

        Console.WriteLine("── DFS (глибина першою) ──");
        foreach (var node in new DfsIterator(root))
        {
            string label = node is LightElementNode el ? $"<{el.TagName}>" : $"\"{node.OuterHTML}\"";
            Console.WriteLine($"  {label}");
        }

        Console.WriteLine("── BFS (ширина першою) ──");
        foreach (var node in new BfsIterator(root))
        {
            string label = node is LightElementNode el ? $"<{el.TagName}>" : $"\"{node.OuterHTML}\"";
            Console.WriteLine($"  {label}");
        }
    }

    static void RunTask9()
    {
        Console.WriteLine("\nMKR_3 Команда (Undo / Redo)");

        var editor = new DomEditor();
        var div = new LightElementNode("div", "block", "double");
        var p = new LightElementNode("p", "block", "double");
        var span = new LightElementNode("span", "inline", "double");

        editor.Execute(new AddChildCommand(div, p));
        editor.Execute(new AddChildCommand(div, span));
        editor.Execute(new AddClassCommand(div, "container"));
        editor.Execute(new AddClassCommand(p, "text-body"));

        Console.WriteLine($"\nHTML після команд:\n{div.OuterHTML}");

        editor.Undo();
        editor.Undo();

        Console.WriteLine($"\nHTML після 2× Undo:\n{div.OuterHTML}");

        editor.Redo();
        Console.WriteLine($"\nHTML після Redo:\n{div.OuterHTML}");

        editor.PrintHistory();
    }

    static void RunTask10()
    {
        Console.WriteLine("\nMKR_4 Стейт (Element States)");

        var button = new StatefulElement("button", "inline", "double");
        button.Add(new LightTextNode("Натисни мене"));

        Console.WriteLine($"Стан: {button.CurrentState}");
        Console.WriteLine(button.OuterHTML);

        button.Hover();
        Console.WriteLine($"Стан: {button.CurrentState} → HTML: {button.OuterHTML}");

        button.Click();
        Console.WriteLine($"Стан: {button.CurrentState} → HTML: {button.OuterHTML}");

        button.Disable();
        Console.WriteLine($"Стан: {button.CurrentState} → HTML: {button.OuterHTML}");

        button.Hover();
        button.Reset();
        Console.WriteLine($"Стан після Reset: {button.CurrentState}");
    }

    static void RunTask11()
    {
        Console.WriteLine("\nMKR_5 Відвідувач (Visitors)");

        var article = new VisitableLightElement("article");
        article.CssClasses.Add("post");

        var h1 = new VisitableLightElement("h1");
        h1.Add(new VisitableLightText("Заголовок статті"));

        var p = new VisitableLightElement("p");
        p.Add(new VisitableLightText("Це текст параграфа з кількома словами для підрахунку."));

        article.Add(h1);
        article.Add(p);

        var renderer = new HtmlRenderVisitor();
        article.Accept(renderer);
        Console.WriteLine($"[HtmlRender] {renderer.GetResult()}");

        var counter = new WordCountVisitor();
        article.Accept(counter);
        counter.PrintReport();

        var validator = new StyleValidatorVisitor();
        article.Accept(validator);
        validator.PrintReport();
    }
}