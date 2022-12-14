namespace AbcSynergy.Synergy
{
    internal sealed class HeroSet
    {
        public List<HeroData> Heroes { get; }
        public int SelectCount { get; }

        public HeroSet(List<HeroData> heroes, int selectCount)
        {
            Heroes = heroes;
            SelectCount = selectCount;
        }
    }
}