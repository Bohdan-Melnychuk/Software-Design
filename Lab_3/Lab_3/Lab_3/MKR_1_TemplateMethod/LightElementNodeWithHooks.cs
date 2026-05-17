using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.MKR_1_TemplateMethod
{
    public abstract class LightElementNodeWithHooks : LightElementNode
    {
        protected LightElementNodeWithHooks(string tagName, string displayType, string closingType)
            : base(tagName, displayType, closingType)
        {
            OnCreated();
        }

        public new void Add(LightNode node)
        {
            OnBeforeInsert(node);
            base.Add(node);
            OnInserted(node);
        }

        public void ApplyStyles(IEnumerable<string> classes)
        {
            OnBeforeStylesApplied();
            CssClasses.AddRange(classes);
            OnStylesApplied();
        }

        protected virtual void OnCreated()                    { }
        protected virtual void OnBeforeInsert(LightNode node) { }
        protected virtual void OnInserted(LightNode node)     { }
        protected virtual void OnBeforeStylesApplied()        { }
        protected virtual void OnStylesApplied()              { }
    }

    public class LoggedLightElement : LightElementNodeWithHooks
    {
        public LoggedLightElement(string tagName,
                                  string displayType = "block",
                                  string closingType = "double")
            : base(tagName, displayType, closingType) { }

        protected override void OnCreated() =>
            Console.WriteLine($"[Lifecycle] <{TagName}> створено");

        protected override void OnBeforeInsert(LightNode node) =>
            Console.WriteLine($"[Lifecycle] <{TagName}> → перед вставкою дочірнього вузла");

        protected override void OnInserted(LightNode node) =>
            Console.WriteLine($"[Lifecycle] <{TagName}> → вузол вставлено (дітей: {ChildrenCount})");

        protected override void OnBeforeStylesApplied() =>
            Console.WriteLine($"[Lifecycle] <{TagName}> → перед застосуванням стилів");

        protected override void OnStylesApplied() =>
            Console.WriteLine($"[Lifecycle] <{TagName}> → стилі застосовано: [{string.Join(", ", CssClasses)}]");
    }
}