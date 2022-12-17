namespace AbcSynergy.Synergy
{
    internal interface IRule
    {
        int Index { get; }
        int Count { get; }
        
        bool CanAddHero(HeroData heroData);
        bool TryAddHero(HeroData heroData);
        List<HeroData> Heroes { get; }
        float MightMultiplier { get; }
        BuffType BuffType { get; }
        bool IsAvailableRule { get; }
        void RemoveAllHeroes();
        void UpdateAvailability();
    }
}