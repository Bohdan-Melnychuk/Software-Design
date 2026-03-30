namespace Lab_3_StructuralPatterns.Task3_Bridge
{
    public interface IRenderer
    {
        void RenderShape(string shapeName);
    }

    public class VectorRenderer : IRenderer
    {
        public void RenderShape(string shapeName)
        {
            System.Console.WriteLine($"Drawing {shapeName} as vectors.");
        }
    }

    public class RasterRenderer : IRenderer
    {
        public void RenderShape(string shapeName)
        {
            System.Console.WriteLine($"Drawing {shapeName} as pixels.");
        }
    }
}