namespace AbcSynergy.Synergy
{
    internal sealed class ClassRule : IRule
    {
        private readonly float _bonusPercent;
        public Class Class { get; }
        public int Index { get; }
        public int Count { get; }
        public List<HeroData> Heroes { get; } = new();
        public float MightMultiplier { get; }
        public BuffType BuffType { get; }
        public bool IsAvailableRule => StaticData.MightyHeroesByClass.TryGetValue(Class, out var list) && list.Count >= Count;

        public ClassRule(
            Class @class,
            int count,
            float bonusPercent,
            BuffType buffType)
        {
            Index = (int) @class;
            Class = @class;
            Count = count;
            _bonusPercent = bonusPercent;
            MightMultiplier = (_bonusPercent + 100) / 100;
            BuffType = buffType;
        }

        public bool CanAddHero(HeroData heroData)
        {
            if (Heroes.Count >= Count)
                return false;

            if (Class == Class.Any || Class == heroData.Class)
            {
                return true;
            }

            return false;
        }

        public bool TryAddHero(HeroData heroData)
        {
            if (!CanAddHero(heroData))
                return false;

            Heroes.Add(heroData);
            return true;
        }

        public void RemoveAllHeroes()
        {
            foreach (HeroData heroData in Heroes)
            {
                heroData.SetUsed(false);
            }

            Heroes.Clear();
        }

        public string ToLongString()
        {
            return $"{Class} #{Count} +{_bonusPercent}% for {BuffType}";
        }

        public override string ToString()
        {
            return $"C_{Class}_{Count}";
        }
    }
}