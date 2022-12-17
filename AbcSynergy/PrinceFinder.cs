using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

internal sealed class PrinceFinder
{
    private readonly MightCalculator _mightCalculator = new();
    private readonly List<HeroData> _heroBuffer;
    private readonly List<HeroData> _activeSet;

    public PrinceFinder()
    {
        _activeSet = new List<HeroData>(StaticData.Heroes.Count);
        _heroBuffer = new List<HeroData>(StaticData.Heroes.Count);
    }

    public void Execute(int squadSize)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var buffer = new List<HeroData>(StaticData.Heroes);
        var heroesCount = buffer.Count;

        var results = new Princes(StaticData.Heroes);

        while (heroesCount > squadSize)
        {
            PrinceData[] sets = results.Buffer;
            results.SetAllResultsDirty();

            foreach (PrinceData resultData in sets)
            {
                _activeSet.Clear();
                _activeSet.AddRange(resultData.Heroes);
                FindNewActiveSet(_activeSet, results);
            }

            heroesCount--;
        }

        var mightTop = new MightTop();

        for (var index = 0; index < results.Buffer.Length; index++)
        {
            PrinceData resultData = results.Buffer[index];
            float calcMight = _mightCalculator.CalcMight(resultData.Heroes);
            mightTop.TryAdd(calcMight, resultData.Heroes);
        }

        stopwatch.Stop();
        mightTop.PrintTop();
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private void FindNewActiveSet(List<HeroData> activeSet, Princes results)
    {
        int maxIndex = activeSet.Count;

        for (int i = 0; i < maxIndex; i++)
        {
            _heroBuffer.Clear();
            _heroBuffer.AddRange(activeSet);
            _heroBuffer.RemoveAt(i);

            float calcMight = _mightCalculator.CalcMight(_heroBuffer);
            results.Add(_heroBuffer, calcMight);
        }
    }
}

internal sealed class Princes
{
    public readonly PrinceData[] Buffer;

    public Princes(List<HeroData> heroes)
    {
        int heroesCount = heroes.Count;
        Buffer = new PrinceData[heroesCount];
        for (var index = 0; index < Buffer.Length; index++)
        {
            var resultData = new PrinceData(index, heroesCount);
            resultData.SetDirty();
            resultData.UpdateResult(heroes, 0);
            Buffer[index] = resultData;
        }
    }

    public void SetAllResultsDirty()
    {
        for (var index = 0; index < Buffer.Length; index++)
        {
            Buffer[index].SetDirty();
        }
    }

    public void Add(List<HeroData> heroes, float might)
    {
        foreach (PrinceData resultData in Buffer)
        {
            resultData.UpdateResult(heroes, might);
        }
    }
}

internal sealed class PrinceData
{
    private readonly int _myHeroIndex;
    private bool _isDirty;

    public List<HeroData> Heroes { get; }
    public float Might { get; private set; }

    public PrinceData(int heroIndex, int maxHeroesCount)
    {
        _myHeroIndex = heroIndex;
        Heroes = new List<HeroData>(maxHeroesCount);
    }

    public void SetDirty()
    {
        _isDirty = true;
    }

    public void UpdateResult(List<HeroData> newHeroes, float newMight)
    {
        for (var i = 0; i < newHeroes.Count; i++)
        {
            if (newHeroes[i].Index == _myHeroIndex)
            {
                if (_isDirty || newMight > Might)
                {
                    Might = newMight;
                    _isDirty = false;
                    Heroes.Clear();
                    Heroes.AddRange(newHeroes);
                }

                return;
            }
        }
    }
}