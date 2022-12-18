using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class WeakLinkFinder
{
    private readonly MightCalculator _mightCalculator = new();
    private List<HeroData> _setBuffer = new List<HeroData>();

    public void Execute(int squadSize, int limit)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var activeSet = new List<HeroData>(StaticData.Heroes);

        var results = new Results(limit);
        results.Add(activeSet, 0);

        var currentHeroesCount = activeSet.Count;
        _setBuffer = new List<HeroData>(currentHeroesCount);

        while (currentHeroesCount > squadSize)
        {
            Console.Write(currentHeroesCount);
            Console.Write(" ");

            List<List<HeroData>> sets = results.GetCurrentResult();
            results.SetDirty();

            for (var index = 0; index < sets.Count; index++)
            {
                List<HeroData> heroes = sets[index];
                FindNewActiveSet(heroes, results);
            }

            currentHeroesCount--;
        }

        var mightTop = new MightTop();

        for (var index = 0; index < results.Top.Count; index++)
        {
            ResultData resultData = results.Top[index];
            mightTop.TryAdd(resultData.Might, resultData.Heroes);
        }

        stopwatch.Stop();
        Console.WriteLine();
        mightTop.PrintTop();
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private void FindNewActiveSet(List<HeroData> activeSet, Results results)
    {
        int maxIndex = activeSet.Count;

        for (int i = 0; i < maxIndex; i++)
        {
            _setBuffer.Clear();
            _setBuffer.AddRange(activeSet);
            _setBuffer.RemoveAt(i);

            float calcMight = _mightCalculator.CalcMight(_setBuffer);
            results.Add(_setBuffer, calcMight);
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

    public void Add(List<HeroData> newHeroes, float newMight)
    {
        for (var index = 0; index < Top.Count; index++)
        {
            ResultData resultData = Top[index];
            if (resultData.HasSame(newHeroes))
                return;
        }
        
        for (var index = 0; index < Top.Count; index++)
        {
            ResultData resultData = Top[index];
            if (resultData.IsDirty)
            {
                resultData.Update(newMight, newHeroes);
                return;
            }
        }

        if (Top.Count < _limit)
        {
            Top.Add(new ResultData(newHeroes, newMight));
        }
        else
        {
            var minimalMight = float.MaxValue;
            ResultData? minimalTop = null;

            for (var i = 0; i < Top.Count; i++)
            {
                ResultData topHero = Top[i];
                if (topHero.Might < minimalMight)
                {
                    minimalMight = topHero.Might;
                    minimalTop = topHero;
                }
            }

            minimalTop?.Update(newMight, newHeroes);
        }
    }

    public List<List<HeroData>> GetCurrentResult()
    {
        return new List<List<HeroData>>(Top.Select(a => new List<HeroData>(a.Heroes)));
    }

    public void SetDirty()
    {
        foreach (ResultData resultData in Top)
        {
            resultData.IsDirty = true;
        }
    }
}

internal sealed class ResultData
{
    public List<HeroData> Heroes { get; }
    public float Might { get; set; }
    public bool IsDirty { get; set; }


    public ResultData(IReadOnlyList<HeroData> heroes, float might)
    {
        Heroes = new List<HeroData>(heroes);
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

    public void Update(float newMight, List<HeroData> newHeroes)
    {
        Might = newMight;
        Heroes.Clear();
        Heroes.AddRange(newHeroes);
        IsDirty = false;
    }
}