using System.Text;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class MightTop
{
    private const int TOP_COUNT = 10;
    private readonly List<TopData> _topHeroes = new();
    private readonly StringBuilder _stringBuilder = new();
    private readonly MightCalculator _mightCalculator = new();

    public void TryAdd(float newMight, IReadOnlyList<HeroData> newHeroes)
    {
        if (_topHeroes.Count < TOP_COUNT)
        {
            _topHeroes.Add(new TopData(newMight, newHeroes));
        }
        else
        {
            for (var i = 0; i < _topHeroes.Count; i++)
            {
                if (IsSameHeroes(_topHeroes[i], newHeroes, newMight))
                    return;
            }

            var minimalMight = float.MaxValue;
            TopData? minimalTop = null;

            for (var i = 0; i < _topHeroes.Count; i++)
            {
                TopData topHero = _topHeroes[i];
                if (topHero.Might < minimalMight)
                {
                    minimalMight = topHero.Might;
                    minimalTop = topHero;
                }
            }

            minimalTop?.Update(newMight, newHeroes);
        }
    }

    private bool IsSameHeroes(TopData topHero, IReadOnlyList<HeroData> newHeroes, float newMight)
    {
        if (topHero.Heroes.Count != newHeroes.Count)
            return false;
        
        if (Math.Abs(topHero.Might - newMight) > 0.1f)
            return false;

        for (var index = 0; index < newHeroes.Count; index++)
        {
            if (newHeroes[index].Index != topHero.Heroes[index].Index)
            {
                return false;
            }
        }

        return true;
    }

    public void PrintTop()
    {
        Console.WriteLine($"Top heroes:");
        
        _topHeroes.Sort((a, b) => b.Might.CompareTo(a.Might));

        foreach (TopData topData in _topHeroes)
        {
            Console.Write($"{topData.Might:F0}: ");
            Console.Write(PrintHeroes(topData.Heroes));
            Console.Write(" (");
            Console.Write(PrintRules(topData.Heroes));
            Console.WriteLine(")");
        }
    }

    private string PrintRules(List<HeroData> heroes)
    {
        _mightCalculator.CalcMight(heroes);

        bool needSeparator = false;

        foreach (ClassRule classRule in StaticData.ClassRules)
            if (classRule.IsUsedInCalculation)
            {
                if (needSeparator)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(classRule);
                needSeparator = true;
            }

        foreach (RaceRule raceRule in StaticData.RaceRules)
            if (raceRule.IsUsedInCalculation)
            {
                if (needSeparator)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(raceRule);
                needSeparator = true;
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