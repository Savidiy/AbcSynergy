namespace AbcSynergy.Synergy
{
    internal sealed class RaceRule : IRule
    {
        private readonly float _bonusPercent;
        public Race Race { get; }
        public int Index { get; }
        public int Count { get; }
        public bool IsUsedInCalculation { get; set; }
        public bool IsRuleForAnyHeroes { get; }
        public List<HeroData> Heroes { get; } = new();
        public float MightMultiplier { get; }
        public BuffType BuffType { get; }
        public bool IsAvailableRule { get; private set; }

        public RaceRule(
            Race race,
            int count,
            float bonusPercent,
            BuffType buffType)
        {
            Index = (int) race;
            Race = race;
            IsRuleForAnyHeroes = Race == Race.Any;
            Count = count;
            _bonusPercent = bonusPercent;
            MightMultiplier = (_bonusPercent + 100) / 100;
            BuffType = buffType;
        }

        public bool CanAddHero(HeroData heroData)
        {
            if (Heroes.Count >= Count)
                return false;

            if (IsRuleForAnyHeroes || Race == heroData.Race)
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

        public void UpdateAvailability()
        {
            IsAvailableRule = StaticData.MightyHeroesByRace.TryGetValue(Race, out var list) && list.Count >= Count;
        }

        public string ToLongString()
        {
            return $"{Race} #{Count} +{_bonusPercent}% for {BuffType}";
        }

        public override string ToString()
        {
            return $"R_{Race}_{Count}";
        }
    }
}