using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.MKR_3_Command
{
    public interface IDomCommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }

    public class AddChildCommand : IDomCommand
    {
        private readonly LightElementNode _parent;
        private readonly LightNode _child;

        public AddChildCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child  = child;
        }

        public string Description => $"AddChild <{(_child is LightElementNode e ? e.TagName : "text")}> to <{_parent.TagName}>";
        public void Execute() => _parent.Add(_child);
        public void Undo()    => _parent.Remove(_child);
    }

    public class RemoveChildCommand : IDomCommand
    {
        private readonly LightElementNode _parent;
        private readonly LightNode _child;

        public RemoveChildCommand(LightElementNode parent, LightNode child)
        {
            _parent = parent;
            _child  = child;
        }

        public string Description => $"RemoveChild <{(_child is LightElementNode e ? e.TagName : "text")}> from <{_parent.TagName}>";
        public void Execute() => _parent.Remove(_child);
        public void Undo()    => _parent.Add(_child);
    }

    public class AddClassCommand : IDomCommand
    {
        private readonly LightElementNode _element;
        private readonly string _className;

        public AddClassCommand(LightElementNode element, string className)
        {
            _element   = element;
            _className = className;
        }

        public string Description => $"AddClass '{_className}' to <{_element.TagName}>";
        public void Execute() => _element.CssClasses.Add(_className);
        public void Undo()    => _element.CssClasses.Remove(_className);
    }

    public class DomEditor
    {
        private readonly Stack<IDomCommand> _undoStack = new();
        private readonly Stack<IDomCommand> _redoStack = new();

        public void Execute(IDomCommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            Console.WriteLine($"[Command] Виконано: {command.Description}");
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) { Console.WriteLine("[Command] Нічого скасовувати"); return; }
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            Console.WriteLine($"[Command] Скасовано: {command.Description}");
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) { Console.WriteLine("[Command] Нічого повторювати"); return; }
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            Console.WriteLine($"[Command] Повторено: {command.Description}");
        }

        public void PrintHistory()
        {
            Console.WriteLine($"[Command] Історія: {_undoStack.Count} дій, {_redoStack.Count} для redo");
        }
    }
}