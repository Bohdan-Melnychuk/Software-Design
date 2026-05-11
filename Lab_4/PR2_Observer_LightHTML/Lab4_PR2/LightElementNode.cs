using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4_PR2
{
    public delegate void EventHandler(object sender, EventArgs args);

    public class EventArgs : System.EventArgs
    {
        public string Type { get; set; }
        public object Data { get; set; }
    }

    public interface IEventTarget
    {
        void AddEventListener(string eventType, EventHandler handler);
        void RemoveEventListener(string eventType, EventHandler handler);
        void DispatchEvent(EventArgs e);
    }

    public class LightElementNode : LightNode, IEventTarget
    {
        public string TagName { get; private set; }
        public string DisplayType { get; private set; }
        public string ClosingType { get; private set; }
        public List<string> CssClasses { get; private set; }

        private List<LightNode> _children;
        private Dictionary<string, List<EventHandler>> _eventListeners;

        public LightElementNode(string tagName, string displayType, string closingType)
        {
            TagName = tagName;
            DisplayType = displayType;
            ClosingType = closingType;
            CssClasses = new List<string>();
            _children = new List<LightNode>();
            _eventListeners = new Dictionary<string, List<EventHandler>>();
        }

        public void Add(LightNode node)
        {
            _children.Add(node);
        }

        public int ChildrenCount
        {
            get { return _children.Count; }
        }

        public void AddEventListener(string eventType, EventHandler handler)
        {
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<EventHandler>();
            }
            _eventListeners[eventType].Add(handler);
        }

        public void RemoveEventListener(string eventType, EventHandler handler)
        {
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(handler);
            }
        }

        public void DispatchEvent(EventArgs e)
        {
            if (_eventListeners.ContainsKey(e.Type))
            {
                foreach (var handler in _eventListeners[e.Type])
                {
                    handler(this, e);
                }
            }
        }

        public void SimulateEvent(string eventType)
        {
            EventArgs args = new EventArgs();
            args.Type = eventType;
            args.Data = $"Подія '{eventType}' на елементі <{TagName}>";
            DispatchEvent(args);
        }

        public override string InnerHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();
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
                {
                    return $"<{TagName}{classes}/>";
                }

                return $"<{TagName}{classes}>{InnerHTML}</{TagName}>";
            }
        }
    }
}