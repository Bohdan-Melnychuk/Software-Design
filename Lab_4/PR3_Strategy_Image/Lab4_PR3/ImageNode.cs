namespace Lab4_PR3
{
    public class ImageNode : LightElementNode
    {
        private IImageLoadStrategy _strategy;
        private string _href;

        public ImageNode(string href, IImageLoadStrategy strategy)
            : base("img", "inline", "single")
        {
            _href = href;
            _strategy = strategy;
            LoadImage();
        }

        private void LoadImage()
        {
            string result = _strategy.Load(_href);
            this.Add(new LightTextNode(result));
        }

        public void SetStrategy(IImageLoadStrategy strategy)
        {
            _strategy = strategy;
            LoadImage();
        }
    }
}