namespace AbcSynergy.Synergy
{
    internal sealed class HeroData
    {
        public string Id { get; }
        public int Index { get; private set; }
        public Race Race { get; }
        public Class Class { get; }
        public int Might { get; private set; }
        public bool CanHaveMana { get; private set; }
        public int DamagePerSecond { get; private set; }
        public float ModifiedMight { get; set; }
        public bool IsUsed { get; private set; }

        public HeroData(
            string id,
            Race race,
            Class @class)
        {
            Id = id;
            Race = race;
            Class = @class;
        }

        public void SetRandom(int might, bool canHaveMana, int damagePerSecond)
        {
            DamagePerSecond = damagePerSecond;
            CanHaveMana = canHaveMana;
            Might = might;
        }

        public void SetUsed(bool isUsed)
        {
            IsUsed = isUsed;
        }

        public override string ToString()
        {
            return $"{Id} (Might={Might}, DPS={DamagePerSecond}, {Race}, {Class}{(CanHaveMana ? ", have mana" : "")})";
        }

        public void SetIndex(int index)
        {
            Index = index;
        }
    }
}