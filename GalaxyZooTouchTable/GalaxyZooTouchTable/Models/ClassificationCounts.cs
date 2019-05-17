namespace GalaxyZooTouchTable.Models
{
    public class ClassificationCounts
    {
        public int Total { get; set; }
        public int Smooth { get; set; }
        public int Features { get; set; }
        public int Star { get; set; }

        public ClassificationCounts(int total, int smooth, int features, int star)
        {
            Total = total;
            Smooth = smooth;
            Features = features;
            Star = star;
        }
    }
}
