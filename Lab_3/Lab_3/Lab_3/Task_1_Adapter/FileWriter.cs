using System.IO;

namespace Lab_3_StructuralPatterns.Task1_Adapter
{
    public class FileWriter
    {
        private readonly string _filePath;

        public FileWriter(string filePath)
        {
            _filePath = filePath;
        }

        public void Write(string text)
        {
            File.AppendAllText(_filePath, text);
        }

        public void WriteLine(string text)
        {
            File.AppendAllLines(_filePath, new[] { text });
        }
    }
}