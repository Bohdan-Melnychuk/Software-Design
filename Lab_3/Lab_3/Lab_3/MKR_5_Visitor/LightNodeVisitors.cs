using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.MKR_5_Visitor
{
    public interface ILightNodeVisitor
    {
        void VisitElement(VisitableLightElement element);
        void VisitText(VisitableLightText text);
    }

    public interface IVisitable
    {
        void Accept(ILightNodeVisitor visitor);
    }

    public class VisitableLightElement : LightElementNode, IVisitable
    {
        private readonly List<LightNode> _childNodes = new();

        public VisitableLightElement(string tagName,
                                     string displayType = "block",
                                     string closingType = "double")
            : base(tagName, displayType, closingType) { }

        public new void Add(LightNode node)
        {
            _childNodes.Add(node);
            base.Add(node);
        }

        public IReadOnlyList<LightNode> ChildNodes => _childNodes;

        public void Accept(ILightNodeVisitor visitor)
        {
            visitor.VisitElement(this);
            foreach (var child in _childNodes)
                if (child is IVisitable v) v.Accept(visitor);
        }
    }

    public class VisitableLightText : LightTextNode, IVisitable
    {
        public VisitableLightText(string text) : base(text) { }

        public void Accept(ILightNodeVisitor visitor) =>
            visitor.VisitText(this);
    }

    public class HtmlRenderVisitor : ILightNodeVisitor
    {
        private readonly System.Text.StringBuilder _sb = new();

        public void VisitElement(VisitableLightElement element)
        {
            string classes = element.CssClasses.Any()
                ? $" class=\"{string.Join(" ", element.CssClasses)}\""
                : "";
            _sb.Append($"<{element.TagName}{classes}>");
        }

        public void VisitText(VisitableLightText text) =>
            _sb.Append(text.OuterHTML);

        public string GetResult() => _sb.ToString();
    }

    public class WordCountVisitor : ILightNodeVisitor
    {
        public int TotalWords { get; private set; }
        public int TotalChars { get; private set; }

        public void VisitElement(VisitableLightElement element) { }

        public void VisitText(VisitableLightText text)
        {
            TotalChars += text.OuterHTML.Length;
            TotalWords += text.OuterHTML
                .Split(new[] { ' ', '\n', '\r', '\t' },
                       StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public void PrintReport() =>
            Console.WriteLine($"[Visitor] Слів: {TotalWords}, Символів: {TotalChars}");
    }

    public class StyleValidatorVisitor : ILightNodeVisitor
    {
        private readonly List<string> _warnings = new();

        public void VisitElement(VisitableLightElement element)
        {
            if (!element.CssClasses.Any())
                _warnings.Add($"<{element.TagName}> не має CSS класів");
        }

        public void VisitText(VisitableLightText text) { }

        public void PrintReport()
        {
            if (_warnings.Count == 0)
            {
                Console.WriteLine("[Visitor] Валідація пройдена: всі елементи мають CSS класи");
                return;
            }
            Console.WriteLine($"[Visitor] Знайдено {_warnings.Count} попереджень:");
            foreach (var w in _warnings)
                Console.WriteLine($"{w}");
        }
    }
}