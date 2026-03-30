using System.Collections.Generic;

namespace Lab_3_StructuralPatterns.Task6_Flyweight
{
    public class LightElementType
    {
        public string TagName { get; }
        public string DisplayType { get; }
        public string ClosingType { get; }

        public LightElementType(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
        }
    }

    public class LightElementFactory
    {
        private readonly Dictionary<string, LightElementType> _types = new();

        public LightElementType GetType(string tagName, string displayType, string closingType)
        {
            string key = $"{tagName}_{displayType}_{closingType}";
            if (!_types.ContainsKey(key))
            {
                _types[key] = new LightElementType(tagName, displayType, closingType);
            }
            return _types[key];
        }
    }
}