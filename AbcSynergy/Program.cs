using AbcSynergy;
using AbcSynergy.Synergy;

const int SQUAD_SIZE = 8;

var randomSeed = int.MinValue;
do
{
    Console.Write("Please enter random seed: ");
    string? readLine = Console.ReadLine();
    if (readLine != null && int.TryParse(readLine, out int result))
        randomSeed = result;
} while (randomSeed == int.MinValue);

var heroesCount = int.MinValue;
do
{
    Console.Write($"Enter heroes count (max {StaticData.Heroes.Count}, Enter to skip): ");
    string? readLine = Console.ReadLine();
    if (readLine != null && readLine == "")
        heroesCount = StaticData.Heroes.Count;

    if (readLine != null && int.TryParse(readLine, out int result))
        heroesCount = result;
} while (heroesCount == int.MinValue);

StaticData.SetRandomSeed(randomSeed);
StaticData.LeaveManyHeroes(heroesCount);
StaticData.PrintHeroes();
StaticData.PrintRules();

Console.WriteLine("\nStart weak link algorithm.");
var weakLinkFinder = new WeakLinkFinder();
weakLinkFinder.Execute(randomSeed, SQUAD_SIZE, 100);

Console.WriteLine("\nStart class cross race algorithm.");
var oracle = new ClassCrossRaceOracle();
oracle.Execute(randomSeed, SQUAD_SIZE);