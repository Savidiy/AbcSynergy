using AbcSynergy.Synergy;

namespace AbcSynergy;

internal class TopData
{
    public float Might { get; private set; }
    public List<HeroData> Heroes { get; }

    public TopData(float might, IReadOnlyList<HeroData> heroes)
    {
        Might = might;
        Heroes = new List<HeroData>(heroes);
    }

    public void Update(float newMight, IReadOnlyList<HeroData> newHeroes)
    {
        Might = newMight;
        Heroes.Clear();
        Heroes.AddRange(newHeroes);
    }
}