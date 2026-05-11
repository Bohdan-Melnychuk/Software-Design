using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4_PR3
{
    public class LightElementNode : LightNode
    {
        public string TagName { get; private set; }
        public string DisplayType { get; private set; }
        public string ClosingType { get; private set; }
        public List<string> CssClasses { get; private set; }

        private List<LightNode> _children;

        public LightElementNode(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
            CssClasses = new List<string>();
            _children = new List<LightNode>();
        }

        public void Add(LightNode node)
        {
            _children.Add(node);
        }

        public int ChildrenCount
        {
            get { return _children.Count; }
        }

        public override string InnerHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
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
                {
                    return $"<{TagName}{classes}/>";
                }

                return $"<{TagName}{classes}>{InnerHTML}</{TagName}>";
            }
        }
    }
}