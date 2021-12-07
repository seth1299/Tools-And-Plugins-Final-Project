namespace Zork
{
    public class Item
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsTakeable { get; }

        public bool AlreadyTaken { get; set; }

        public int ScoreValue { get; }

        public Item(string name, string description, bool isTakeable, int scoreValue)
        {
            Name = name;
            Description = description;
            IsTakeable = isTakeable;
            AlreadyTaken = false;
            ScoreValue = scoreValue;
        }

        public override string ToString() => Name;
    }
}
