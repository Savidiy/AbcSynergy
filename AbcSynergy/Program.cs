
using AbcSynergy;
const int SQUAD_SIZE = 8;
const int RANDOM_SEED = 125;

var weakLinkFinder = new WeakLinkFinder();
weakLinkFinder.Execute(RANDOM_SEED, SQUAD_SIZE);
//
// var oracle = new Oracle();
// oracle.Execute(RANDOM_SEED, SQUAD_SIZE);


//
// Console.WriteLine("Hello, World!");
// var taskReader = new TaskReader();
// GameData gameData = taskReader.ReadData("task1.txt");
//
// public class TaskReader
// {
//     public GameData ReadData(string fileName)
//     {
//         foreach (string line in System.IO.File.ReadLines(fileName))
//         {  
//             System.Console.WriteLine(line);  
//             counter++;  
//         }
//     }
// }

