using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class WeakLinkFinder
{
    private readonly MightCalculator _mightCalculator = new();
    private readonly List<List<HeroData>> _setBuffer = new List<List<HeroData>>();

    public void Execute(int squadSize, int limit)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var activeSet = new List<HeroData>(StaticData.Heroes);

        var results = new Results(limit);
        results.Add(activeSet, 0);

        var currentHeroesCount = activeSet.Count;

        for (int i = 0; i < currentHeroesCount; i++)
        {
            _setBuffer.Add(new List<HeroData>(currentHeroesCount));
        }

        while (currentHeroesCount > squadSize)
        {
            List<List<HeroData>> sets = results.GetCurrentResult();
            results.Clear();

            foreach (List<HeroData> heroes in sets)
            {
                FindNewActiveSet(heroes, results);
            }

            currentHeroesCount--;
        }

        var mightTop = new MightTop();

        for (var index = 0; index < 5; index++)
        {
            ResultData resultData = results.Top[index];
            float calcMight = _mightCalculator.CalcMight(resultData.Heroes);
            mightTop.TryAdd(calcMight, resultData.Heroes);
        }
        
        stopwatch.Stop();
        mightTop.PrintTop();
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private void FindNewActiveSet(List<HeroData> activeSet, Results results)
    {
        int maxIndex = activeSet.Count;

        for (int i = 0; i < maxIndex; i++)
        {
            _setBuffer[i].Clear();
            _setBuffer[i].AddRange(activeSet);
            _setBuffer[i].RemoveAt(i);
        }

        for (int i = 0; i < maxIndex; i++)
        {
            float calcMight = _mightCalculator.CalcMight(_setBuffer[i]);
            results.Add(_setBuffer[i], calcMight);
        }
    }
}

internal sealed class Results
{
    public readonly List<ResultData> Top = new();
    private readonly int _limit;

    public Results(int limit)
    {
        _limit = limit;
    }

    public void Add(List<HeroData> heroes, float might)
    {
        foreach (ResultData resultData in Top)
        {
            if (resultData.HasSame(heroes))
                return;
        }

        Top.Add(new ResultData(new List<HeroData>(heroes), might));

        if (Top.Count >= _limit)
        {
            Top.Sort((a, b) => b.Might.CompareTo(a.Might));
            Top.RemoveAt(Top.Count - 1);
        }
    }

    public List<List<HeroData>> GetCurrentResult()
    {
        return new List<List<HeroData>>(Top.Select(a => a.Heroes));
    }

    public void Clear()
    {
        Top.Clear();
    }
}

internal sealed class ResultData
{
    public List<HeroData> Heroes { get; }
    public float Might { get; }

    public ResultData(List<HeroData> heroes, float might)
    {
        Heroes = heroes;
        Might = might;
    }

    public bool HasSame(List<HeroData> heroes)
    {
        if (heroes.Count != Heroes.Count)
            return false;

        for (var index = 0; index < Heroes.Count; index++)
        {
            if (Heroes[index] != heroes[index])
                return false;
        }

        return true;
    }
}