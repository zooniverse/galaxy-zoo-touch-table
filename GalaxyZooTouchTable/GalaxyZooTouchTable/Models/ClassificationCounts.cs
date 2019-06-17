namespace GalaxyZooTouchTable.Models
{
    public class ClassificationCounts
    {
        public int Total { get; set; }
        public int Smooth { get; set; }
        public int Features { get; set; }
        public int Star { get; set; }

        public ClassificationCounts(int total = 0, int smooth = 0, int features = 0, int star = 0)
        {
            Total = total;
            Smooth = smooth;
            Features = features;
            Star = star;
        }
    }
}
