namespace AbcSynergy.Synergy
{
    internal sealed class HeroSet
    {
        public List<HeroData> Heroes { get; private set; }
        public int SelectCount { get; private set; }

        public void Set(List<HeroData> heroes, int selectCount)
        {
            Heroes = heroes;
            SelectCount = selectCount;
        }
    }
}