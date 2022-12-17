using System.Diagnostics;

namespace AbcSynergy.Synergy;

internal sealed class HeroBruteForceChecker
{
    private readonly MightCalculator _mightCalculator = new MightCalculator();
    private readonly MightTop _top = new MightTop();

    public void Execute(int squadSize)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        int heroesCount = StaticData.Heroes.Count;
        var heroes = new HeroData[squadSize];
        var counters = new List<int>(squadSize);
        for (int i = 0; i < squadSize; i++)
            counters.Add(i);

        long repeatsCount = 1;
        for (int i = heroesCount - squadSize + 1; i <= heroesCount; i++)
        {
            repeatsCount *= i;
        }
        for (int i = 2; i <= squadSize; i++)
        {
            repeatsCount /= i;
        }

        Console.WriteLine($"Planned {repeatsCount} iterations. Progress in %:");
        long repeatCounterLimit = Math.Max(repeatsCount / 1000, 1);
        long repeatCounter = 0;
        int tenCounter = 0;


        do
        {
            for (var index = 0; index < counters.Count; index++)
            {
                int counter = counters[index];
                heroes[index] = StaticData.Heroes[counter];
            }

            float calcMight = _mightCalculator.CalcMight(heroes);
            _top.TryAdd(calcMight, heroes);

            repeatCounter++;
            if (repeatCounter % repeatCounterLimit == 0)
            {
                tenCounter++;
                if (tenCounter % 10 == 0)
                {
                    Console.Write(tenCounter / 10);
                }
                else
                {
                    Console.Write('.');
                }
            }
        } while (TryUpdateIndex(counters, heroesCount));


        stopwatch.Stop();
        Console.WriteLine();
        _top.PrintTop();
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private bool TryUpdateIndex(List<int> counters, int heroesCount)
    {
        counters[^1]++;

        for (var i = counters.Count - 1; i >= 0; i--)
        {
            int maxValueForCounter = heroesCount - counters.Count + i;
            if (i == 0)
            {
                if (counters[i] > maxValueForCounter)
                    return false;
            }
            else
            {
                if (counters[i] > maxValueForCounter)
                {
                    int seniorIndex = counters[i - 1];
                    seniorIndex++;
                    counters[i - 1] = seniorIndex;

                    for (int minorIndex = i; minorIndex < counters.Count; minorIndex++)
                    {
                        seniorIndex++;
                        counters[minorIndex] = seniorIndex;
                    }
                }
            }
        }

        return true;
    }
}