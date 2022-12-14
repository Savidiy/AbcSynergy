using System.Collections.Generic;

namespace TriggersSystem.Tests.Synergy
{
    internal sealed class RaceRule : IRule
    {
        public Race Race { get; }
        public int Index { get; }
        public int Count { get; }
        public List<HeroData> Heroes { get; } = new();
        public float MightMultiplier { get; }
        public BuffType BuffType { get; }

        public RaceRule(
            Race race,
            int count,
            float bonusPercent,
            BuffType buffType)
        {
            Index = (int) race;
            Race = race;
            Count = count;
            MightMultiplier = (bonusPercent + 100) / 100;
            BuffType = buffType;
        }

        public bool CanAddHero(HeroData heroData)
        {
            if (Heroes.Count >= Count)
                return false;

            if (Race == Race.Any || Race == heroData.Race)
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

        public override string ToString()
        {
            return $"R_{Race}_{Count}";
        }
    }
}