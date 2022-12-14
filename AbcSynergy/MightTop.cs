using System.Text;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class MightTop
{
    private const int TOP_COUNT = 5;
    private readonly List<TopData> _topHeroes = new();
    private readonly StringBuilder _stringBuilder = new();

    public void TryAdd(float might, IReadOnlyList<HeroData> heroesBuffer, RulesSet classRules, RulesSet raceRules)
    {
        if (_topHeroes.Count < TOP_COUNT)
        {
            _topHeroes.Add(new TopData(might, heroesBuffer, classRules, raceRules));
            _topHeroes.Sort(MightDecreaseComparison);
        }
        else if (_topHeroes[^1].Might < might)
        {
            _topHeroes[^1] = new TopData(might, heroesBuffer, classRules, raceRules);
            _topHeroes.Sort(MightDecreaseComparison);
        }
    }

    private int MightDecreaseComparison(TopData x, TopData y)
    {
        return y.Might.CompareTo(x.Might);
    }

    public void PrintTop()
    {
        Console.WriteLine("Top heroes:");

        foreach (TopData topData in _topHeroes)
        {
            Console.Write($"{topData.Might:F0}: ");
            Console.Write(PrintHeroes(topData.Heroes));
            Console.Write(" (");
            Console.Write(PrintRules(topData.ClassRules));
            Console.Write(", ");
            Console.Write(PrintRules(topData.RaceRules));
            Console.WriteLine(")");
        }
    }

    private string PrintRules(RulesSet rulesSet)
    {
        int rulesCount = rulesSet.Rules.Count;
        for (var index = 0; index < rulesCount; index++)
        {
            if (index != 0)
                _stringBuilder.Append(", ");

            IRule rule = rulesSet.Rules[index];
            _stringBuilder.Append(rule);
        }

        var result = _stringBuilder.ToString();
        _stringBuilder.Clear();
        return result;
    }

    private string PrintHeroes(List<HeroData> heroes)
    {
        for (var index = 0; index < heroes.Count; index++)
        {
            if (index != 0)
                _stringBuilder.Append(", ");

            HeroData heroData = heroes[index];
            _stringBuilder.Append(heroData.Id);
        }

        var result = _stringBuilder.ToString();
        _stringBuilder.Clear();
        return result;
    }
}