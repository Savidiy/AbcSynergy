using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class MightCalculator
{
    private readonly Dictionary<Class, int> _heroesOfClass = new();
    private readonly Dictionary<Race, int> _heroesOfRace = new();
    private readonly List<Class> _usedClasses = new();
    private readonly List<Race> _usedRaces = new();

    public MightCalculator()
    {
        foreach (Class value in Enum.GetValues(typeof(Class)))
            _heroesOfClass.Add(value, 0);

        foreach (Race value in Enum.GetValues(typeof(Race)))
            _heroesOfRace.Add(value, 0);
    }

    public float CalcMight(IReadOnlyList<HeroData> heroes)
    {
        CollectStatistic(heroes);

        for (int index = StaticData.ClassRules.Count - 1; index >= 0; index--)
        {
            ClassRule classRule = StaticData.ClassRules[index];
            Class ruleClass = classRule.Class;
            if (_usedClasses.Contains(ruleClass))
                continue;

            if (_heroesOfClass[ruleClass] < classRule.Count)
                continue;

            _usedClasses.Add(ruleClass);
            AddMight(heroes, classRule);
        }

        for (int index = StaticData.RaceRules.Count - 1; index >= 0; index--)
        {
            RaceRule raceRule = StaticData.RaceRules[index];
            Race ruleClass = raceRule.Race;
            if (_usedRaces.Contains(ruleClass))
                continue;

            if (_heroesOfRace[ruleClass] < raceRule.Count)
                continue;

            _usedRaces.Add(ruleClass);
            AddMight(heroes, raceRule);
        }

        float might = 0f;
        foreach (HeroData heroData in heroes)
        {
            might += heroData.ModifiedMight;
            // heroData.ModifiedMight = heroData.Might;
        }

        return might;
    }

    private void CollectStatistic(IReadOnlyList<HeroData> heroes)
    {
        _usedClasses.Clear();
        _usedRaces.Clear();
        foreach (Class value in _heroesOfClass.Keys)
            _heroesOfClass[value] = 0;

        foreach (Race value in _heroesOfRace.Keys)
            _heroesOfRace[value] = 0;

        foreach (HeroData heroData in heroes)
        {
            _heroesOfClass[heroData.Class]++;
            _heroesOfRace[heroData.Race]++;
            heroData.ModifiedMight = heroData.Might;
        }
    }

    private void AddMight(IReadOnlyList<HeroData> heroes, IRule rule)
    {
        switch (rule.BuffType)
        {
            case BuffType.All:
                for (var index = 0; index < heroes.Count; index++)
                {
                    HeroData heroData = heroes[index];
                    heroData.ModifiedMight *= rule.MightMultiplier;
                }

                break;
            case BuffType.OnlyMyType:
                for (var index = 0; index < heroes.Count; index++)
                {
                    HeroData heroData = heroes[index];
                    if (rule is ClassRule classRule && classRule.Class == heroData.Class ||
                        rule is RaceRule raceRule && raceRule.Race == heroData.Race)
                        heroData.ModifiedMight *= rule.MightMultiplier;
                }

                break;
            case BuffType.MostDanger:
                HeroData mostDangerHero = null;
                float maxDamagePerSeconds = float.MinValue;
                for (var index = 0; index < heroes.Count; index++)
                {
                    HeroData ruleHero = heroes[index];
                    if (ruleHero.DamagePerSecond > maxDamagePerSeconds)
                    {
                        mostDangerHero = ruleHero;
                        maxDamagePerSeconds = ruleHero.DamagePerSecond;
                    }
                }

                mostDangerHero.ModifiedMight *= rule.MightMultiplier;

                break;
            case BuffType.CanHaveMana:
                foreach (HeroData heroData in heroes)
                    if (heroData.CanHaveMana)
                        heroData.ModifiedMight *= rule.MightMultiplier;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}