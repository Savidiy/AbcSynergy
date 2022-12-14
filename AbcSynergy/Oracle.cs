using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

public class Oracle
{
    private const int SQUAD_SIZE = 8;

    public void Execute()
    {
        StaticData.UpdateHeroesMight(125);
        List<RulesSet> classCombinations = GetClassCombinations();
        List<RulesSet> raceCombinations = GetRaceCombinations();
        var heroesCombinator = new HeroesCombinator();
        List<HeroData> heroesBuffer = new(SQUAD_SIZE);
        var mightTop = new MightTop();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var classIndex = 0; classIndex < classCombinations.Count; classIndex++)
        {
            RulesSet classRules = classCombinations[classIndex];
            for (var index = 0; index < raceCombinations.Count; index++)
            {
                RulesSet raceRules = raceCombinations[index];

                float might = FindBestHeroes(classRules, raceRules, heroesCombinator, heroesBuffer);
                mightTop.TryAdd(might, heroesBuffer, classRules, raceRules);
            }

            Console.Clear();
            Console.WriteLine($"{classIndex}/{classCombinations.Count}");

            mightTop.PrintTop();
        }

        // assert
        stopwatch.Stop();
        StaticData.PrintHeroes();
        StaticData.PrintRules();
        Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private List<RulesSet> GetClassCombinations()
    {
        List<RulesSet> fullSets = GetRulesSets(StaticData.ClassRules, StaticData.WithoutClassRules);
        // fullSets.Count.Should().Be(1165);
        return fullSets;
    }

    private List<RulesSet> GetRaceCombinations()
    {
        List<RulesSet> fullSets = GetRulesSets(StaticData.RaceRules, StaticData.WithoutRaceRules);
        // fullSets.Count.Should().Be(1195);
        return fullSets;
    }

    private float FindBestHeroes(RulesSet classRules,
        RulesSet raceRules,
        HeroesCombinator heroesCombinator,
        List<HeroData> heroesBuffer)
    {
        int classCombinations = FindRuleCombinationsCount(classRules);
        int raceCombinations = FindRuleCombinationsCount(raceRules);

        RulesSet minimalRules = classCombinations < raceCombinations ? classRules : raceRules;
        RulesSet maximalRules = classCombinations >= raceCombinations ? classRules : raceRules;

        heroesBuffer.Clear();
        float bestMight = 0;
        int bestMightFounded = 0;
        foreach (IReadOnlyList<HeroData> combination in heroesCombinator.GetHeroesCombinations(minimalRules))
        {
            if (TryAddAllHeroes(combination, minimalRules, maximalRules))
            {
                if (TryAddEmptyHeroes(minimalRules, maximalRules))
                {
                    float might = CalcMight(minimalRules, maximalRules);
                    if (might > bestMight)
                    {
                        bestMight = might;
                        UpdateBestHeroes(heroesBuffer, minimalRules);
                        bestMightFounded++;
                    }
                }
            }

            minimalRules.RemoveAllHeroes();
            maximalRules.RemoveAllHeroes();
        }

        // if (bestMightFounded > 1)
        //     Console.Write($"{bestMightFounded} ");

        return bestMight;
    }

    private void UpdateBestHeroes(List<HeroData> bestHeroes, RulesSet minimalRules)
    {
        bestHeroes.Clear();
        foreach (IRule rule in minimalRules.Rules)
        {
            foreach (HeroData ruleHero in rule.Heroes)
            {
                bestHeroes.Add(ruleHero);
            }
        }
    }

    private bool TryAddEmptyHeroes(RulesSet minimalRules, RulesSet maximalRules)
    {
        if (minimalRules.IsFullHeroes)
            return true;

        foreach (HeroData hero in StaticData.MightyHeroes)
        {
            if (hero.IsUsed)
                continue;

            if (minimalRules.CanAddHero(hero) && maximalRules.CanAddHero(hero))
            {
                minimalRules.TryAddHero(hero);
                maximalRules.TryAddHero(hero);

                if (minimalRules.IsFullHeroes)
                    return true;
            }
        }

        return minimalRules.IsFullHeroes;
    }

    private float CalcMight(RulesSet minimalRules, RulesSet maximalRules)
    {
        minimalRules.ResetHeroMight();

        minimalRules.ModifyMightForPrivateRules();
        maximalRules.ModifyMightForPrivateRules();

        UseMostDangerRule(minimalRules, maximalRules);

        float mightMultiplier = minimalRules.GetCommonMightMultiplier();
        mightMultiplier *= maximalRules.GetCommonMightMultiplier();

        float sumMight = CalcSumMight(minimalRules);

        return sumMight * mightMultiplier;
    }

    private static float CalcSumMight(RulesSet minimalRules)
    {
        float sumMight = 0;
        foreach (IRule rule in minimalRules.Rules)
        {
            foreach (HeroData ruleHero in rule.Heroes)
            {
                sumMight += ruleHero.ModifiedMight;
            }
        }

        return sumMight;
    }

    private static void UseMostDangerRule(RulesSet minimalRules, RulesSet maximalRules)
    {
        if (!minimalRules.HasMostDangerRule() && !maximalRules.HasMostDangerRule())
            return;

        float mostDangerMightMultiplier = minimalRules.GetModifyMightForMostDangerRule();
        mostDangerMightMultiplier *= maximalRules.GetModifyMightForMostDangerRule();

        HeroData mightyHero = null;
        float maxMight = float.MinValue;
        foreach (IRule rule in minimalRules.Rules)
        {
            foreach (HeroData ruleHero in rule.Heroes)
            {
                if (ruleHero.ModifiedMight > maxMight)
                {
                    mightyHero = ruleHero;
                    maxMight = ruleHero.ModifiedMight;
                }
            }
        }

        mightyHero.ModifiedMight *= mostDangerMightMultiplier;
    }

    private static bool TryAddAllHeroes(IReadOnlyList<HeroData> combination, RulesSet minimalRules, RulesSet maximalRules)
    {
        foreach (HeroData heroData in combination)
        {
            heroData.SetUsed(true);
            minimalRules.TryAddHero(heroData);
            if (!maximalRules.TryAddHero(heroData))
                return false;
        }

        return true;
    }

    private int FindRuleCombinationsCount(RulesSet rulesSet)
    {
        var count = 1;
        foreach (IRule rule in rulesSet.Rules)
        {
            if (rule is ClassRule classRule && classRule.Class != Class.Any)
            {
                int heroesCount = StaticData.MightyHeroesByClass[classRule.Class].Count;
                for (int i = 0; i < classRule.Count; i++)
                {
                    count *= heroesCount - i;
                }
            }
            else if (rule is RaceRule raceRule && raceRule.Race != Race.Any)
            {
                int heroesCount = StaticData.MightyHeroesByRace[raceRule.Race].Count;
                for (int i = 0; i < raceRule.Count; i++)
                {
                    count *= heroesCount - i;
                }
            }
        }

        return count;
    }

    private List<RulesSet> GetRulesSets(IReadOnlyList<IRule> rules, IReadOnlyList<IRule> withoutRules)
    {
        var fullSets = new List<RulesSet>();
        var buffer = new List<RulesSet>();

        var growingSets = new List<RulesSet>();
        growingSets.Add(new(SQUAD_SIZE));

        while (growingSets.Count > 0)
        {
            foreach (RulesSet growingSet in growingSets)
            {
                FillBufferWithNewRules(buffer, growingSet, rules);

                RulesSet completedNewRulesSet = CreateRuleWithAnyInstance(withoutRules, growingSet);
                buffer.Add(completedNewRulesSet);
            }

            growingSets.Clear();

            DecomposeBufferToFullAndGrowingSets(buffer, fullSets, growingSets);

            buffer.Clear();
        }

        return fullSets;
    }

    private static void DecomposeBufferToFullAndGrowingSets(List<RulesSet> buffer,
        List<RulesSet> fullSets,
        List<RulesSet> growingSets)
    {
        foreach (RulesSet rulesSet in buffer)
        {
            if (rulesSet.RemainingCount == 0)
            {
                fullSets.Add(rulesSet);
            }
            else
            {
                growingSets.Add(rulesSet);
            }
        }
    }

    private static RulesSet CreateRuleWithAnyInstance(IReadOnlyList<IRule> withoutRules, RulesSet rulesSet)
    {
        int remainingCount = rulesSet.RemainingCount;
        RulesSet completedNewRulesSet = rulesSet.Clone();
        completedNewRulesSet.Add(withoutRules[remainingCount]);
        return completedNewRulesSet;
    }

    private static void FillBufferWithNewRules(List<RulesSet> bufferSets, RulesSet rulesSet, IReadOnlyList<IRule> rules)
    {
        int lastIndex = rulesSet.LastIndex;

        foreach (IRule rule in rules)
        {
            if (lastIndex > rule.Index)
                continue;
            
            if (!rule.IsAvailableRule)
                continue;
                
            if (!rulesSet.CanAdd(rule))
                continue;

            RulesSet newRulesSet = rulesSet.Clone();
            newRulesSet.Add(rule);
            bufferSets.Add(newRulesSet);
        }
    }
}