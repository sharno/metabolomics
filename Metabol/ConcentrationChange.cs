namespace Metabol
{
    public class ConcentrationChange
    {
        public string Name { get; set; }
        public int Change { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}