using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab_3_StructuralPatterns.Task5_Composite
{
    public class LightElementNode : LightNode
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }
        public List<string> CssClasses { get; } = new List<string>();
        
        private readonly List<LightNode> _children = new List<LightNode>();

        public LightElementNode(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }

        public void Add(LightNode node) => _children.Add(node);
        public int ChildrenCount => _children.Count;

        public override string InnerHTML
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var child in _children)
                {
                    sb.Append(child.OuterHTML);
                    sb.Append(Environment.NewLine);
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
    }
}