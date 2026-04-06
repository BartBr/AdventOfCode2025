using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using AdventOfCode2025.Common;
using AdventOfCode2025.Console;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
	.AddUserSecrets<Program>(optional: true, reloadOnChange: false)
	.Build();
var yourName = config["YourName"] ?? throw new ValidationException("Could not find a 'YourName' key in appsettings.json");

Console.WriteLine($"Hello {yourName}");

var activatedPuzzleRecords = HappyPuzzleHelpers
	.DiscoverPuzzles(false, yourName)
	.Select(yourPuzzle => new ActivatorRecord(yourPuzzle.Name, Activator.CreateInstance(yourPuzzle)!));

var client = new HttpClient
{
	BaseAddress = new Uri(config["BaseUrl"] ?? throw new ValidationException("Could not find a 'BaseUrl' key in appsettings.json"))
};

foreach (var puzzleRecord in activatedPuzzleRecords)
{
	Console.WriteLine($"=== {puzzleRecord.Name} ({puzzleRecord.ActivatedPuzzle.GetType().FullName}) ".PadRight(80, '='));

	Console.WriteLine("Reading input");
	var input = Helpers.GetInput(puzzleRecord.Name, yourName);
	var day = int.Parse(puzzleRecord.Name.AsSpan(3));
	using var content = new StringContent(
		input.Text,
		Encoding.UTF8,
		"text/plain");

	using (var response = await client.PostAsync($"/solve/2025/{day}/1", content))
	{
		Console.WriteLine("Solving part 1...");
		var part1 = puzzleRecord.SolvePart1(input).ToString();
		Console.WriteLine(part1);
		var confirmedPart1 = await response.Content.ReadAsStringAsync();
		Debug.Assert(part1 == confirmedPart1);
	}

	using (var response = await client.PostAsync($"/solve/2025/{day}/2", content))
	{
		Console.WriteLine("\nSolving part 2...");
		var part2 = puzzleRecord.SolvePart2(input).ToString();
		Console.WriteLine(part2);
		var confirmedPart2 = await response.Content.ReadAsStringAsync();
		Debug.Assert(part2 == confirmedPart2);

		Console.WriteLine();
	}
}