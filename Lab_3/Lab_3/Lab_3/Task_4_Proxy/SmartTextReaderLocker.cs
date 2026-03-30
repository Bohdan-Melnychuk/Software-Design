using System;
using System.Text.RegularExpressions;

namespace Lab_3_StructuralPatterns.Task4_Proxy
{
    public class SmartTextReaderLocker : ITextReader
    {
        private readonly ITextReader _reader;
        private readonly Regex _lockPattern;

        public SmartTextReaderLocker(ITextReader reader, string pattern)
        {
            _reader = reader;
            _lockPattern = new Regex(pattern);
        }

        public char[][] ReadText(string filePath)
        {
            if (_lockPattern.IsMatch(filePath))
            {
                Console.WriteLine($"[Locker] Access denied to file: {filePath}!");
                return null;
            }

            return _reader.ReadText(filePath);
        }
    }
}