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

StaticData.Initialize(randomSeed, heroesCount);
StaticData.PrintHeroes();
StaticData.PrintRules();

do
{
    var iterations = 3000;
    var temperatureDegradationMultiplier = 0.9997f;
    var cycles = 50;
    var simulatedAnnealingMethod = new SimulatedAnnealingMethod();
    Console.WriteLine(
        $"\n   Start simulated annealing algorithm iterations = {iterations}, koef = {temperatureDegradationMultiplier}, cycles = {cycles}");

    simulatedAnnealingMethod.Execute(SQUAD_SIZE, iterations, temperatureDegradationMultiplier, cycles);
} while (Console.ReadKey().Key == ConsoleKey.Enter);

// var limit = 100;
// var weakLinkFinder = new WeakLinkFinder();
// Console.WriteLine($"\n   Start weak link algorithm with limit = {limit}");
// weakLinkFinder.Execute(SQUAD_SIZE, limit);

// Console.WriteLine("\n   Start prince find algorithm");
// var princeFinder = new PrinceFinder();
// princeFinder.Execute(SQUAD_SIZE);

// Console.WriteLine("\n   Start brute force algorithm");
// var heroBruteForceChecker = new HeroBruteForceChecker();
// heroBruteForceChecker.Execute(SQUAD_SIZE);

// Console.WriteLine("\n   Start class cross race algorithm");
// var oracle = new ClassCrossRaceOracle();
// oracle.Execute(SQUAD_SIZE);

// Console.WriteLine("Press 'Enter' key to close");
// bool needRepeat = true;
// do
// {
//     ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(intercept: true);
//
//     if (consoleKeyInfo.Key == ConsoleKey.Enter)
//         needRepeat = false;
// } while (needRepeat);