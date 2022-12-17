using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

public class ClassCrossRaceOracle
{
    private const int MAX_RULES_SIMULTANEOUSLY = 4;
    private readonly MightCalculator _mightCalculator = new();
    private HeroData[] _mightHeroesBuffer;
    private int _squadSize;

    public void Execute(int randomSeed, int squadSize)
    {
        _squadSize = squadSize;
        _mightHeroesBuffer = new HeroData[squadSize];
        List<RulesSet> classCombinations = GetClassCombinations();
        List<RulesSet> raceCombinations = GetRaceCombinations();
        var heroesCombinator = new HeroesCombinator(MAX_RULES_SIMULTANEOUSLY, squadSize);
        List<HeroData> heroesBuffer = new(squadSize);
        var mightTop = new MightTop();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Console.WriteLine(
            $"Class combinations {classCombinations.Count} * race combinations {raceCombinations.Count} for {squadSize} heroes, random seed {randomSeed}");

        for (var classIndex = 0; classIndex < classCombinations.Count; classIndex++)
        {
            RulesSet classRules = classCombinations[classIndex];
            for (var index = 0; index < raceCombinations.Count; index++)
            {
                RulesSet raceRules = raceCombinations[index];

                float might = FindBestHeroes(classRules, raceRules, heroesCombinator, heroesBuffer);
                mightTop.TryAdd(might, heroesBuffer, classRules, raceRules);
            }

            if (classIndex % 10 == 0)
                Console.Write(classIndex);
            else
                Console.Write('.');
        }

        // assert
        stopwatch.Stop();
        Console.WriteLine();
        mightTop.PrintTop(squadSize);
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
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

        heroesCombinator.SetupRules(minimalRules);

        while (heroesCombinator.HasNextCombination)
        {
            IReadOnlyList<HeroData> combination = heroesCombinator.GetNextCombination();

            if (TryAddAllHeroes(combination, minimalRules, maximalRules))
            {
                if (TryAddEmptyHeroes(minimalRules, maximalRules))
                {
                    float might = _mightCalculator.CalcMight(GetHeroes(minimalRules));
                    float notFullRulesMight = CalcMight(minimalRules, maximalRules);
                    if (might - notFullRulesMight < 1 && might > bestMight)
                    {
                        bestMight = might;
                        UpdateBestHeroes(heroesBuffer, minimalRules);
                    }
                }
            }

            minimalRules.RemoveAllHeroes();
            maximalRules.RemoveAllHeroes();
        }

        return bestMight;
    }

    private IReadOnlyList<HeroData> GetHeroes(RulesSet rulesSet)
    {
        int bufferIndex = 0;
        for (var ruleIndex = 0; ruleIndex < rulesSet.Rules.Count; ruleIndex++)
        {
            IRule rule = rulesSet.Rules[ruleIndex];
            for (var heroIndex = 0; heroIndex < rule.Heroes.Count; heroIndex++)
            {
                HeroData hero = rule.Heroes[heroIndex];
                _mightHeroesBuffer[bufferIndex] = hero;
                bufferIndex++;
            }
        }

        return _mightHeroesBuffer;
    }

    private void UpdateBestHeroes(List<HeroData> bestHeroes, RulesSet minimalRules)
    {
        bestHeroes.Clear();
        for (var ruleIndex = 0; ruleIndex < minimalRules.Rules.Count; ruleIndex++)
        {
            IRule rule = minimalRules.Rules[ruleIndex];
            for (var heroIndex = 0; heroIndex < rule.Heroes.Count; heroIndex++)
            {
                HeroData ruleHero = rule.Heroes[heroIndex];
                bestHeroes.Add(ruleHero);
            }
        }
    }

    private bool TryAddEmptyHeroes(RulesSet minimalRules, RulesSet maximalRules)
    {
        if (minimalRules.IsFullHeroes)
            return true;

        for (var index = 0; index < StaticData.MightyHeroes.Count; index++)
        {
            HeroData hero = StaticData.MightyHeroes[index];
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
        UseCanHaveManaRule(minimalRules, maximalRules);

        float mightMultiplier = minimalRules.CommonMightMultiplier;
        mightMultiplier *= maximalRules.CommonMightMultiplier;

        float sumMight = CalcSumMight(minimalRules);

        return sumMight * mightMultiplier;
    }

    private static float CalcSumMight(RulesSet minimalRules)
    {
        float sumMight = 0;
        for (var index = 0; index < minimalRules.Rules.Count; index++)
        {
            IRule rule = minimalRules.Rules[index];
            for (var heroIndex = 0; heroIndex < rule.Heroes.Count; heroIndex++)
            {
                HeroData ruleHero = rule.Heroes[heroIndex];
                sumMight += ruleHero.ModifiedMight;
            }
        }

        return sumMight;
    }

    private static void UseMostDangerRule(RulesSet minimalRules, RulesSet maximalRules)
    {
        if (!minimalRules.HasMostDangerRule && !maximalRules.HasMostDangerRule)
            return;

        float mostDangerMightMultiplier = minimalRules.MultiplierForMostDangerRule;
        mostDangerMightMultiplier *= maximalRules.MultiplierForMostDangerRule;

        HeroData mostDangerHero = null;
        float maxDamagePerSeconds = float.MinValue;
        foreach (IRule rule in minimalRules.Rules)
        {
            foreach (HeroData ruleHero in rule.Heroes)
            {
                if (ruleHero.DamagePerSecond > maxDamagePerSeconds)
                {
                    mostDangerHero = ruleHero;
                    maxDamagePerSeconds = ruleHero.DamagePerSecond;
                }
            }
        }

        mostDangerHero.ModifiedMight *= mostDangerMightMultiplier;
    }

    private static void UseCanHaveManaRule(RulesSet minimalRules, RulesSet maximalRules)
    {
        if (!minimalRules.HasCanHaveManaRule && !maximalRules.HasCanHaveManaRule)
            return;

        float canHaveManaMightMultiplier = minimalRules.MultiplierForCanHaveManaRule;
        canHaveManaMightMultiplier *= maximalRules.MultiplierForCanHaveManaRule;

        foreach (IRule rule in minimalRules.Rules)
        {
            foreach (HeroData ruleHero in rule.Heroes)
            {
                if (ruleHero.CanHaveMana)
                {
                    ruleHero.ModifiedMight *= canHaveManaMightMultiplier;
                }
            }
        }
    }

    private static bool TryAddAllHeroes(IReadOnlyList<HeroData> combination, RulesSet minimalRules, RulesSet maximalRules)
    {
        for (var index = 0; index < combination.Count; index++)
        {
            HeroData heroData = combination[index];
            if (!maximalRules.TryAddHero(heroData))
                return false;
        }

        for (var index = 0; index < combination.Count; index++)
        {
            HeroData heroData = combination[index];
            heroData.SetUsed(true);
            minimalRules.TryAddHero(heroData);
        }

        return true;
    }

    private int FindRuleCombinationsCount(RulesSet rulesSet)
    {
        var count = 1;
        for (var index = 0; index < rulesSet.Rules.Count; index++)
        {
            IRule rule = rulesSet.Rules[index];
            if (rule is ClassRule classRule && !classRule.IsRuleForAnyHeroes)
            {
                int heroesCount = StaticData.MightyHeroesByClass[classRule.Class].Count;
                for (int i = 0; i < classRule.Count; i++)
                {
                    count *= heroesCount - i;
                }
            }
            else if (rule is RaceRule raceRule && !raceRule.IsRuleForAnyHeroes)
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
        growingSets.Add(new(_squadSize));

        while (growingSets.Count > 0)
        {
            for (var index = 0; index < growingSets.Count; index++)
            {
                RulesSet growingSet = growingSets[index];
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
        for (var index = 0; index < buffer.Count; index++)
        {
            RulesSet rulesSet = buffer[index];
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

        for (var index = 0; index < rules.Count; index++)
        {
            IRule rule = rules[index];
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