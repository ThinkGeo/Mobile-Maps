namespace GettingStartedSample
{
    public class CellModel
    {
        public string Name { get; private set; }

        public CellModel(string name)
        {
            Name = name;
        }
    }
}