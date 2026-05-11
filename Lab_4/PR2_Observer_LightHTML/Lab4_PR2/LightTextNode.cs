namespace Lab4_PR2
{
    public class LightTextNode : LightNode
    {
        private string _text;

        public LightTextNode(string text)
        {
            _text = text;
        }

        public override string InnerHTML
        {
            get { return _text; }
        }

        public override string OuterHTML
        {
            get { return _text; }
        }
    }
}