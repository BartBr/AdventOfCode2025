using AdventOfCode2025.Common;

namespace AdventOfCode2025.Console;

public record struct ActivatorRecord(string Name, object ActivatedPuzzle)
{
	public object SolvePart1(Input input) => ActivatedPuzzle
		.GetType()
		.GetMethod(nameof(IHappyPuzzle<object,object>.SolvePart1))?
		.Invoke(ActivatedPuzzle, [input]) ?? throw new InvalidOperationException();

	public object SolvePart2(Input input) => ActivatedPuzzle
		.GetType()
		.GetMethod(nameof(IHappyPuzzle<object,object>.SolvePart2))?
		.Invoke(ActivatedPuzzle, [input]) ?? throw new InvalidOperationException();
}