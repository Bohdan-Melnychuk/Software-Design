using System.IO;

namespace Lab4_PR3
{
    public class FileSystemLoadStrategy : IImageLoadStrategy
    {
        public string Load(string href)
        {
            if (File.Exists(href))
            {
                return $"[Файл знайдено: {href}]";
            }
            else
            {
                return $"[Файл не знайдено: {href}]";
            }
        }
    }
}