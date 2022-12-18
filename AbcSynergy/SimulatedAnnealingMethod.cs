using System.Diagnostics;
using AbcSynergy.Synergy;

namespace AbcSynergy;

public class SimulatedAnnealingMethod
{
    private readonly MightCalculator _mightCalculator = new();
    private readonly Random _random = new Random();

    public void Execute(int squadSize, int iterations, float temperatureDegradationMultiplier, int cycles)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var mightTop = new MightTop();
        var nextHeroes = new List<HeroData>(squadSize);
        var potentialHeroes = new List<HeroData>(StaticData.Heroes.Count);


        for (int i = 0; i < cycles; i++)
        {
            List<HeroData> currentHeroes = GetRandomHeroes(squadSize);
            float currentHeroesMight = _mightCalculator.CalcMight(currentHeroes);
            float temperature = StaticData.MightyHeroes[0].Might;

            for (var iterationIndex = 0; iterationIndex < iterations; iterationIndex++)
            {
                temperature *= temperatureDegradationMultiplier;
                potentialHeroes.Clear();
                potentialHeroes.AddRange(StaticData.Heroes);

                foreach (HeroData heroData in currentHeroes)
                {
                    potentialHeroes.Remove(heroData);
                }

                int randomPositionIndex = _random.Next(0, squadSize);
                int randomNewHeroIndex = _random.Next(0, potentialHeroes.Count);

                nextHeroes.Clear();
                nextHeroes.AddRange(currentHeroes);
                nextHeroes[randomPositionIndex] = potentialHeroes[randomNewHeroIndex];

                float nextHeroesMight = _mightCalculator.CalcMight(nextHeroes);

                if (nextHeroesMight > currentHeroesMight || NeedMakeTransition(currentHeroesMight, nextHeroesMight, temperature))
                {
                    currentHeroes[randomPositionIndex] = potentialHeroes[randomNewHeroIndex];
                    // PrintInfo(iterationIndex, currentHeroesMight: nextHeroesMight, previousHeroesMight: currentHeroesMight,
                    //     currentHeroes);

                    currentHeroesMight = nextHeroesMight;

                    mightTop.TryAdd(currentHeroesMight, currentHeroes);
                }
            }
        }

        stopwatch.Stop();
        Console.WriteLine();
        mightTop.PrintTop();
        Console.WriteLine($"\nElapsed {stopwatch.ElapsedMilliseconds} mils");
    }

    private static void PrintInfo(int iterationIndex, float currentHeroesMight, float previousHeroesMight,
        List<HeroData> currentHeroes)
    {
        Console.ForegroundColor = previousHeroesMight > currentHeroesMight ? ConsoleColor.Red : ConsoleColor.Gray;
        Console.Write($"#{iterationIndex}: {currentHeroesMight:F0} ");
        Console.ForegroundColor = ConsoleColor.Gray;
        for (var index = 0; index < currentHeroes.Count; index++)
        {
            if (index != 0)
                Console.Write(", ");

            HeroData currentHero = currentHeroes[index];
            Console.Write(currentHero.Id);
        }

        Console.WriteLine();
    }

    private bool NeedMakeTransition(float currentHeroesMight, float nextHeroesMight, float temperature)
    {
        float deltaMight = nextHeroesMight - currentHeroesMight;
        float power = deltaMight / temperature;
        double transitionProbability = Math.Exp(power);
        // Console.WriteLine($"{transitionProbability:F3} ");
        double nextDouble = _random.NextDouble();
        return nextDouble < transitionProbability;
    }

    private List<HeroData> GetRandomHeroes(int squadSize)
    {
        var randomHeroes = new List<HeroData>(squadSize);
        var potentialHeroes = new List<HeroData>(StaticData.Heroes);

        for (int i = 0; i < squadSize; i++)
        {
            int potentialHeroesCount = potentialHeroes.Count;
            int selectedPotentialHeroIndex = _random.Next(0, potentialHeroesCount);
            randomHeroes.Add(potentialHeroes[selectedPotentialHeroIndex]);
            potentialHeroes.RemoveAt(selectedPotentialHeroIndex);
        }

        return randomHeroes;
    }
}