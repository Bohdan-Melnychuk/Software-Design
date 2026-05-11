using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MKR_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("МКР №1: Поведінкові шаблони\n");

            // 1. Iterator
            Console.WriteLine("1. Iterator");
            
            var root = new LightElementNode("div", "block", "double");
            var h1 = new LightElementNode("h1", "block", "double");
            var p1 = new LightElementNode("p", "block", "double");
            var p2 = new LightElementNode("p", "block", "double");
            
            h1.Add(new LightTextNode("Заголовок"));
            p1.Add(new LightTextNode("Перший параграф"));
            p2.Add(new LightTextNode("Другий параграф"));
            
            root.Add(h1);
            root.Add(p1);
            root.Add(p2);
            
            Console.WriteLine("Обхід в глибину (DFS):");
            var dfs = new DepthFirstIterator(root);
            while (dfs.HasNext())
            {
                var node = dfs.Next();
                Console.WriteLine($"  - {node.GetType().Name}");
            }
            
            Console.WriteLine("Обхід в ширину (BFS):");
            var bfs = new BreadthFirstIterator(root);
            while (bfs.HasNext())
            {
                var node = bfs.Next();
                Console.WriteLine($"  - {node.GetType().Name}");
            }
            
            // 2. Command
            Console.WriteLine("\n2. Command");
            
            var invoker = new CommandInvoker();
            var container = new LightElementNode("div", "block", "double");
            
            var addCommand = new AddNodeCommand(container, new LightTextNode("Новий текст"));
            invoker.ExecuteCommand(addCommand);
            Console.WriteLine($"Після додавання: {container.InnerHTML}");
            
            invoker.Undo();
            Console.WriteLine($"Після undo: {container.InnerHTML}");
            
            invoker.Redo();
            Console.WriteLine($"Після redo: {container.InnerHTML}");
            
            // 3. State
            Console.WriteLine("\n3. State");
            
            var nodeWithState = new StatefulElementNode("button", "inline", "double");
            nodeWithState.SetState(new CreatedState());
            nodeWithState.Render();
            
            nodeWithState.SetState(new InsertedState());
            nodeWithState.Render();
            
            nodeWithState.SetState(new RemovedState());
            nodeWithState.Render();
            
            // 4. Template Method
            Console.WriteLine("\n4. Template Method");
            
            var hookNode = new HookElementNode("section", "block", "double");
            hookNode.Add(new LightTextNode("Контент з хуками"));
            Console.WriteLine(hookNode.OuterHTML);
            
            // 5. Visitor
            Console.WriteLine("\n5. Visitor");
            
            var visitableRoot = new LightElementNode("html", "block", "double");
            var body = new LightElementNode("body", "block", "double");
            var div1 = new LightElementNode("div", "block", "double");
            var span = new LightElementNode("span", "inline", "double");
            
            div1.Add(new LightTextNode("Текст в div"));
            span.Add(new LightTextNode("Текст в span"));
            body.Add(div1);
            body.Add(span);
            visitableRoot.Add(body);
            
            var countVisitor = new NodeCountVisitor();
            visitableRoot.Accept(countVisitor);
            Console.WriteLine($"Кількість елементів: {countVisitor.Count}");
            
            var tagCollector = new TagNameCollectorVisitor();
            visitableRoot.Accept(tagCollector);
            Console.WriteLine($"Теги: {string.Join(", ", tagCollector.Tags)}");
            
            Console.WriteLine("\nНатисніть Enter для виходу...");
            Console.ReadLine();
        }
    }

    // БАЗОВІ КЛАСИ LightHTML
    
    public abstract class LightNode
    {
        public abstract string InnerHTML { get; }
        public abstract string OuterHTML { get; }
        
        public abstract void Accept(IVisitor visitor);
    }
    
    public class LightTextNode : LightNode
    {
        private string _text;
        
        public LightTextNode(string text)
        {
            _text = text;
        }
        
        public override string InnerHTML => _text;
        public override string OuterHTML => _text;
        
        public override void Accept(IVisitor visitor)
        {
            visitor.VisitTextNode(this);
        }
    }
    
    public class LightElementNode : LightNode
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }
        public List<string> CssClasses { get; } = new List<string>();
        
        private List<LightNode> _children = new List<LightNode>();
        
        public LightElementNode(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }
        
        public virtual void Add(LightNode node)
        {
            _children.Add(node);
        }
        
        public void Remove(LightNode node)
        {
            _children.Remove(node);
        }
        
        public IReadOnlyList<LightNode> GetChildren()
        {
            return _children.AsReadOnly();
        }
        
        public int ChildrenCount => _children.Count;
        
        public override string InnerHTML
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var child in _children)
                {
                    sb.Append(child.OuterHTML);
                }
                return sb.ToString();
            }
        }
        
        public override string OuterHTML
        {
            get
            {
                string classes = CssClasses.Any() ? $" class=\"{string.Join(" ", CssClasses)}\"" : "";
                
                if (ClosingType == "single")
                    return $"<{TagName}{classes}/>";
                    
                return $"<{TagName}{classes}>{InnerHTML}</{TagName}>";
            }
        }
        
        public override void Accept(IVisitor visitor)
        {
            visitor.VisitElementNode(this);
            foreach (var child in _children)
            {
                child.Accept(visitor);
            }
        }
    }

    // 1. ITERATOR
    
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
    }
    
    public class DepthFirstIterator : IIterator<LightNode>
    {
        private Stack<LightNode> _stack = new Stack<LightNode>();
        
        public DepthFirstIterator(LightNode root)
        {
            _stack.Push(root);
        }
        
        public bool HasNext()
        {
            return _stack.Count > 0;
        }
        
        public LightNode Next()
        {
            if (!HasNext()) return null;
            
            var current = _stack.Pop();
            
            if (current is LightElementNode element)
            {
                var children = element.GetChildren();
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    _stack.Push(children[i]);
                }
            }
            
            return current;
        }
    }
    
    public class BreadthFirstIterator : IIterator<LightNode>
    {
        private Queue<LightNode> _queue = new Queue<LightNode>();
        
        public BreadthFirstIterator(LightNode root)
        {
            _queue.Enqueue(root);
        }
        
        public bool HasNext()
        {
            return _queue.Count > 0;
        }
        
        public LightNode Next()
        {
            if (!HasNext()) return null;
            
            var current = _queue.Dequeue();
            
            if (current is LightElementNode element)
            {
                foreach (var child in element.GetChildren())
                {
                    _queue.Enqueue(child);
                }
            }
            
            return current;
        }
    }

    // 2. COMMAND
    
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    
    public class AddNodeCommand : ICommand
    {
        private LightElementNode _parent;
        private LightNode _child;
        
        public AddNodeCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child = child;
        }
        
        public void Execute()
        {
            _parent.Add(_child);
        }
        
        public void Undo()
        {
            _parent.Remove(_child);
        }
    }
    
    public class CommandInvoker
    {
        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();
        
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }
        
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }
        
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }
    }

    // 3. STATE
    
    public interface INodeState
    {
        void Render(LightElementNode node);
    }
    
    public class CreatedState : INodeState
    {
        public void Render(LightElementNode node)
        {
            Console.WriteLine($"Елемент <{node.TagName}> створений, але ще не в DOM");
        }
    }
    
    public class InsertedState : INodeState
    {
        public void Render(LightElementNode node)
        {
            Console.WriteLine($"Елемент <{node.TagName}> вставлений в DOM");
        }
    }
    
    public class RemovedState : INodeState
    {
        public void Render(LightElementNode node)
        {
            Console.WriteLine($"Елемент <{node.TagName}> видалений з DOM");
        }
    }
    
    public class StatefulElementNode : LightElementNode
    {
        private INodeState _state;
        
        public StatefulElementNode(string tagName, string displayType, string closingType)
            : base(tagName, displayType, closingType)
        {
            _state = new CreatedState();
        }
        
        public void SetState(INodeState state)
        {
            _state = state;
        }
        
        public void Render()
        {
            _state.Render(this);
        }
    }

    // 4. TEMPLATE METHOD
    
    public abstract class LifecycleNode : LightElementNode
    {
        public LifecycleNode(string tagName, string displayType, string closingType)
            : base(tagName, displayType, closingType)
        {
            OnCreated();
        }
        
        protected abstract void OnCreated();
        protected virtual void OnInserted(LightNode child) { }
        protected virtual void OnRemoved(LightNode child) { }
        protected virtual void OnTextRendered() { }
        
        public override void Add(LightNode node)
        {
            base.Add(node);
            OnInserted(node);
        }
        
        public void RemoveNode(LightNode node)
        {
            Remove(node);
            OnRemoved(node);
        }
        
        public void RenderText()
        {
            OnTextRendered();
        }
    }
    
    public class HookElementNode : LifecycleNode
    {
        public HookElementNode(string tagName, string displayType, string closingType)
            : base(tagName, displayType, closingType)
        {
        }
        
        protected override void OnCreated()
        {
            Console.WriteLine($"Хук: Елемент {TagName} створений");
        }
        
        protected override void OnInserted(LightNode child)
        {
            Console.WriteLine($"Хук: Дочірній елемент доданий до {TagName}");
        }
        
        protected override void OnRemoved(LightNode child)
        {
            Console.WriteLine($"Хук: Дочірній елемент видалений з {TagName}");
        }
        
        protected override void OnTextRendered()
        {
            Console.WriteLine($"Хук: Текст відрендерений в {TagName}");
        }
    }

    // 5. VISITOR
    
    public interface IVisitor
    {
        void VisitElementNode(LightElementNode node);
        void VisitTextNode(LightTextNode node);
    }
    
    public class NodeCountVisitor : IVisitor
    {
        public int Count { get; private set; } = 0;
        
        public void VisitElementNode(LightElementNode node)
        {
            Count++;
        }
        
        public void VisitTextNode(LightTextNode node)
        {
            Count++;
        }
    }
    
    public class TagNameCollectorVisitor : IVisitor
    {
        public List<string> Tags { get; } = new List<string>();
        
        public void VisitElementNode(LightElementNode node)
        {
            Tags.Add(node.TagName);
        }
        
        public void VisitTextNode(LightTextNode node)
        {
            Tags.Add("#text");
        }
    }
}