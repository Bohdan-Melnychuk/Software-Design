using Lab_3_StructuralPatterns.Task5_Composite;

namespace Lab_3_StructuralPatterns.MKR_4_State
{
    public interface IElementState
    {
        string StateName { get; }
        string GetStateClass();
        void OnHover(StatefulElement element);
        void OnClick(StatefulElement element);
        void OnDisable(StatefulElement element);
        void OnReset(StatefulElement element);
    }

    public class DefaultState : IElementState
    {
        public string StateName => "Default";
        public string GetStateClass() => "";

        public void OnHover(StatefulElement el)   => el.TransitionTo(new HoveredState());
        public void OnClick(StatefulElement el)   => el.TransitionTo(new ActiveState());
        public void OnDisable(StatefulElement el) => el.TransitionTo(new DisabledState());
        public void OnReset(StatefulElement el)   => Console.WriteLine("[State] Вже у стані Default");
    }

    public class HoveredState : IElementState
    {
        public string StateName => "Hovered";
        public string GetStateClass() => "hover";

        public void OnHover(StatefulElement el)   => Console.WriteLine("[State] Вже наведено (Hovered)");
        public void OnClick(StatefulElement el)   => el.TransitionTo(new ActiveState());
        public void OnDisable(StatefulElement el) => el.TransitionTo(new DisabledState());
        public void OnReset(StatefulElement el)   => el.TransitionTo(new DefaultState());
    }

    public class ActiveState : IElementState
    {
        public string StateName => "Active";
        public string GetStateClass() => "active";

        public void OnHover(StatefulElement el)   => el.TransitionTo(new HoveredState());
        public void OnClick(StatefulElement el)   => Console.WriteLine("[State] Вже активний (Active)");
        public void OnDisable(StatefulElement el) => el.TransitionTo(new DisabledState());
        public void OnReset(StatefulElement el)   => el.TransitionTo(new DefaultState());
    }

    public class DisabledState : IElementState
    {
        public string StateName => "Disabled";
        public string GetStateClass() => "disabled";

        public void OnHover(StatefulElement el)   => Console.WriteLine("[State] Елемент відключено, hover ігнорується");
        public void OnClick(StatefulElement el)   => Console.WriteLine("[State] Елемент відключено, клік ігнорується");
        public void OnDisable(StatefulElement el) => Console.WriteLine("[State] Вже відключено (Disabled)");
        public void OnReset(StatefulElement el)   => el.TransitionTo(new DefaultState());
    }

    public class StatefulElement : LightElementNode
    {
        private IElementState _state;

        public StatefulElement(string tagName,
                               string displayType = "block",
                               string closingType = "double")
            : base(tagName, displayType, closingType)
        {
            _state = new DefaultState();
        }

        public void TransitionTo(IElementState newState)
        {
            Console.WriteLine($"[State] <{TagName}>: {_state.StateName} → {newState.StateName}");
            _state = newState;
        }

        public void Hover()   => _state.OnHover(this);
        public void Click()   => _state.OnClick(this);
        public void Disable() => _state.OnDisable(this);
        public void Reset()   => _state.OnReset(this);

        public override string OuterHTML
        {
            get
            {
                string stateClass = _state.GetStateClass();
                if (!string.IsNullOrEmpty(stateClass) && !CssClasses.Contains(stateClass))
                    CssClasses.Add(stateClass);

                string result = base.OuterHTML;

                if (!string.IsNullOrEmpty(stateClass))
                    CssClasses.Remove(stateClass);

                return result;
            }
        }

        public string CurrentState => _state.StateName;
    }
}