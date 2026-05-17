using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.MKR_2_Iterator
{
    public interface ILightNodeIterable
    {
        IEnumerable<LightNode> GetChildNodes();
    }

    public class IterableLightElement : LightElementNode, ILightNodeIterable
    {
        private readonly List<LightNode> _childNodes = new();

        public IterableLightElement(string tagName,
                                    string displayType = "block",
                                    string closingType = "double")
            : base(tagName, displayType, closingType) { }

        public new void Add(LightNode node)
        {
            _childNodes.Add(node);
            base.Add(node);
        }

        public IEnumerable<LightNode> GetChildNodes() => _childNodes;
    }

    public class DfsIterator : IEnumerable<LightNode>
    {
        private readonly LightNode _root;
        public DfsIterator(LightNode root) => _root = root;

        public IEnumerator<LightNode> GetEnumerator()
        {
            var stack = new Stack<LightNode>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                yield return node;

                if (node is ILightNodeIterable iterable)
                    foreach (var child in iterable.GetChildNodes().Reverse())
                        stack.Push(child);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public class BfsIterator : IEnumerable<LightNode>
    {
        private readonly LightNode _root;
        public BfsIterator(LightNode root) => _root = root;

        public IEnumerator<LightNode> GetEnumerator()
        {
            var queue = new Queue<LightNode>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node;

                if (node is ILightNodeIterable iterable)
                    foreach (var child in iterable.GetChildNodes())
                        queue.Enqueue(child);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}