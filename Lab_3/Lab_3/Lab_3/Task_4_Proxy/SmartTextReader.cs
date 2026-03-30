using System.IO;
using System.Linq;

namespace Lab_3_StructuralPatterns.Task4_Proxy
{
    public class SmartTextReader : ITextReader
    {
        public char[][] ReadText(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            
            return lines.Select(line => line.ToCharArray()).ToArray();
        }
    }
}