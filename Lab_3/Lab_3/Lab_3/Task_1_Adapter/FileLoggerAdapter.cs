namespace Lab_3_StructuralPatterns.Task1_Adapter
{
    public class FileLoggerAdapter : Logger
    {
        private readonly FileWriter _fileWriter;

        public FileLoggerAdapter(FileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public new void Log(string message)
        {
            _fileWriter.WriteLine($"[LOG]: {message}");
        }

        public new void Error(string message)
        {
            _fileWriter.WriteLine($"[ERROR]: {message}");
        }

        public new void Warn(string message)
        {
            _fileWriter.WriteLine($"[WARN]: {message}");
        }
    }
}