using AbcSynergy.Synergy;

namespace AbcSynergy;

internal class TopData
{
    public float Might { get; }
    public List<HeroData> Heroes { get; }
    public List<IRule> Rules { get; }

    public TopData(float might, IReadOnlyList<HeroData> heroes)
    {
        Might = might;
        Heroes = new List<HeroData>(heroes);
        Rules = new List<IRule>();
        
        foreach (ClassRule classRule in StaticData.ClassRules)
            if (classRule.IsUsedInCalculation)
                Rules.Add(classRule);

        foreach (RaceRule raceRule in StaticData.RaceRules)
            if (raceRule.IsUsedInCalculation)
                Rules.Add(raceRule);

    }
}