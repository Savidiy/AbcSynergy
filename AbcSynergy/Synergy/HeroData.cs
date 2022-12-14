namespace AbcSynergy.Synergy
{
    internal sealed class HeroData
    {
        public string ID { get; }
        public Race Race { get; }
        public Class Class { get; }
        public int Might { get; private set; }
        public float ModifiedMight { get; set; }
        public bool IsUsed { get; private set; }

        public HeroData(
            string id,
            Race race,
            Class @class)
        {
            ID = id;
            Race = race;
            Class = @class;
        }

        public void SetMight(int might)
        {
            Might = might;
        }

        public override string ToString()
        {
            return $"{ID}({Might}, {Race}, {Class})";
        }

        public void SetUsed(bool isUsed)
        {
            IsUsed = isUsed;
        }
    }
}