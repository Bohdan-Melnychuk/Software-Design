using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2_Patterns.Task4_Prototype
{
    public class Virus : ICloneable
    {
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public List<Virus> Children { get; set; }
        
        public Virus(double weight, int age, string name, string species)
        {
            Weight = weight;
            Age = age;
            Name = name;
            Species = species;
            Children = new List<Virus>();
        }
        
        public void AddChild(Virus child)
        {
            Children.Add(child);
        }
        
        public object Clone()
        {
            Virus clonedVirus = (Virus)this.MemberwiseClone();
            
            clonedVirus.Children = new List<Virus>();
            
            foreach (var child in Children)
            {
                clonedVirus.Children.Add((Virus)child.Clone());
            }
            
            return clonedVirus;
        }
        
        public void DisplayInfo(int level = 0)
        {
            string indent = new string(' ', level * 2);
            Console.WriteLine($"{indent}   Вірус: {Name}");
            Console.WriteLine($"{indent}   Вид: {Species}");
            Console.WriteLine($"{indent}   Вага: {Weight} мкм");
            Console.WriteLine($"{indent}   Вік: {Age} днів");
            
            if (Children.Any())
            {
                Console.WriteLine($"{indent}   Діти:");
                foreach (var child in Children)
                {
                    child.DisplayInfo(level + 1);
                }
            }
        }
    }
}