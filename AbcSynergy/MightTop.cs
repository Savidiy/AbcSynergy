using System.Text;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class MightTop
{
    private const int TOP_COUNT = 10;
    private readonly List<TopData> _topHeroes = new();
    private readonly StringBuilder _stringBuilder = new();

    public void TryAdd(float might, IReadOnlyList<HeroData> heroesBuffer)
    {
        if (_topHeroes.Count < TOP_COUNT)
        {
            _topHeroes.Add(new TopData(might, heroesBuffer));
            _topHeroes.Sort(MightDecreaseComparison);
        }
        else if (might - _topHeroes[^1].Might > 1)
        {
            _topHeroes[^1] = new TopData(might, heroesBuffer);
            _topHeroes.Sort(MightDecreaseComparison);
        }
    }

    private int MightDecreaseComparison(TopData x, TopData y)
    {
        return y.Might.CompareTo(x.Might);
    }

    public void PrintTop()
    {
        Console.WriteLine($"Top heroes:");

        foreach (TopData topData in _topHeroes)
        {
            Console.Write($"{topData.Might:F0}: ");
            Console.Write(PrintHeroes(topData.Heroes));
            Console.Write(" (");
            Console.Write(PrintRules(topData.Rules));
            Console.WriteLine(")");
        }
    }

    private string PrintRules(List<IRule> rules)
    {
        int rulesCount = rules.Count;
        for (var index = 0; index < rulesCount; index++)
        {
            if (index != 0)
                _stringBuilder.Append(", ");

            IRule rule = rules[index];
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