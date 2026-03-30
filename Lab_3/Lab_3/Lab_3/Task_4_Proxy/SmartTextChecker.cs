using System;

namespace Lab_3_StructuralPatterns.Task4_Proxy
{
    public class SmartTextChecker : ITextReader
    {
        private readonly ITextReader _reader;

        public SmartTextChecker(ITextReader reader)
        {
            _reader = reader;
        }

        public char[][] ReadText(string filePath)
        {
            Console.WriteLine($"[Checker] Opening file: {filePath}");
            
            char[][] result = _reader.ReadText(filePath);
            
            Console.WriteLine($"[Checker] Successfully read file: {filePath}");
            int rowCount = result.Length;
            int charCount = 0;
            foreach (var row in result) charCount += row.Length;

            Console.WriteLine($"[Checker] Stats: Rows={rowCount}, Total Characters={charCount}");
            Console.WriteLine($"[Checker] Closing file: {filePath}");
            
            return result;
        }
    }
}