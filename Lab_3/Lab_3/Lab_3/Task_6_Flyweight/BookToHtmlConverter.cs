using System;
using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.Task6_Flyweight
{
    public class BookToHtmlConverter
    {
        private readonly LightElementFactory _factory = new();

        public LightElementNode Convert(string[] bookLines)
        {
            var rootType = _factory.GetType("div", "block", "double");
            var root = new LightElementNode(rootType.TagName, rootType.DisplayType, rootType.ClosingType);

            int actualElementCount = 0;
            
            foreach (var line in bookLines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.Contains("[_Exit_]")) 
                    continue;

                string tagName = "p";

                if (actualElementCount == 0) 
                    tagName = "h1";
                else if (line.Length < 20) 
                    tagName = "h2";
                else if (char.IsWhiteSpace(line[0])) 
                    tagName = "blockquote";

                var type = _factory.GetType(tagName, "block", "double");
                var element = new LightElementNode(type.TagName, type.DisplayType, type.ClosingType);
                
                element.Add(new LightTextNode(line.Trim()));
                root.Add(element);

                actualElementCount++;
            }

            return root;
        }
    }
}