using AbcSynergy.Synergy;

namespace AbcSynergy;

internal class TopData
{
    public float Might { get; }
    public List<HeroData> Heroes { get; }
    public RulesSet ClassRules { get; }
    public RulesSet RaceRules { get; }

    public TopData(float might, IReadOnlyList<HeroData> heroes, RulesSet classRules, RulesSet raceRules)
    {
        Might = might;
        Heroes = new List<HeroData>(heroes);
        ClassRules = classRules;
        RaceRules = raceRules;
    }
}