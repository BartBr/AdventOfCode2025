using System.ComponentModel.DataAnnotations;
using AdventOfCode2025.Common;
using AdventOfCode2025.Console;
using Microsoft.Extensions.Configuration;

var yourName = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
	.AddUserSecrets<Program>(optional: true, reloadOnChange: false)
	.Build()["YourName"] ?? throw new ValidationException("Could not find a 'YourName' key in appsettings.json");

Console.WriteLine($"Hello {yourName}");

var activatedPuzzleRecords = HappyPuzzleHelpers
	.DiscoverPuzzles(true, yourName)
	.Select(yourPuzzle => new ActivatorRecord(yourPuzzle.Name, Activator.CreateInstance(yourPuzzle)!))
	.ToList();

foreach (var puzzleRecord in activatedPuzzleRecords)
{
	Console.WriteLine($"=== {puzzleRecord.Name} ({puzzleRecord.ActivatedPuzzle.GetType().FullName}) ".PadRight(80, '='));

	Console.WriteLine("Reading input");
	var input = Helpers.GetInput(puzzleRecord.Name, yourName);

	Console.WriteLine("Solving part 1...");
	Console.WriteLine(puzzleRecord.SolvePart1(input));

	Console.WriteLine("\nSolving part 2...");
	Console.WriteLine(puzzleRecord.SolvePart2(input));

	Console.WriteLine();
}