namespace AdventOfCode2025.Common;

public static class Helpers
{
	public static Input GetInput(string day, string user)
	{
		var path = Path.Combine("Assets", day, user + ".txt");
		return GetInputForPathInternal(path);
	}

	public static IEnumerable<Input> GetAllInputs(string day)
	{
		var dayFolder = Path.Combine("Assets", day);

		foreach (var inputFilePath in Directory
			         .EnumerateFiles(dayFolder, "*.txt", SearchOption.TopDirectoryOnly)
			         .OrderBy(f => f))
		{
			yield return GetInputForPathInternal(inputFilePath);
		}
	}

	private static Input GetInputForPathInternal(string path)
	{
		var fileName = Path.GetFileName(path);

		var inputText = File.ReadAllText(path);
		var inputLines = File.ReadAllLines(path);
		var inputEnumerable = inputLines.AsEnumerable();
		return new(fileName, inputText, inputLines, inputEnumerable);
	}
}