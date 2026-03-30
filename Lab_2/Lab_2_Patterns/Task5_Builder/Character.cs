using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2_Patterns.Task5_Builder
{
    public class Character
    {
        public string Name { get; set; }
        public string Height { get; set; }
        public string Build { get; set; }
        public string HairColor { get; set; }
        public string EyeColor { get; set; }
        public string Clothing { get; set; }
        public List<string> Inventory { get; set; } = new List<string>();
        public List<string> GoodDeeds { get; set; } = new List<string>();
        public List<string> EvilDeeds { get; set; } = new List<string>();
        public string Alignment { get; set; }
        
        public void ShowInfo()
        {
            Console.WriteLine($"\n{Name} ({Alignment})");
            Console.WriteLine($"Зріст: {Height}");
            Console.WriteLine($"Статура: {Build}");
            Console.WriteLine($"Колір волосся: {HairColor}");
            Console.WriteLine($"Колір очей: {EyeColor}");
            Console.WriteLine($"Одяг: {Clothing}");
            
            Console.WriteLine("Інвентар:");
            foreach (var item in Inventory)
                Console.WriteLine($"  - {item}");
            
            if (GoodDeeds.Count > 0)
            {
                Console.WriteLine("Добрі справи:");
                foreach (var deed in GoodDeeds)
                    Console.WriteLine($"   {deed}");
            }
            
            if (EvilDeeds.Count > 0)
            {
                Console.WriteLine("Злі справи:");
                foreach (var deed in EvilDeeds)
                    Console.WriteLine($"   {deed}");
            }
        }
    }
}