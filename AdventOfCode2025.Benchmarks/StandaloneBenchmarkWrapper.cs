using AdventOfCode2025.Common;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode2025.Benchmarks;

public class StandaloneBenchmarkWrapper<TPuzzle, TReturn> : StandaloneBenchmarkWrapper<TPuzzle, TReturn, TReturn>
	where TPuzzle : IHappyPuzzle<TReturn, TReturn>, new();

public class StandaloneBenchmarkWrapper<TPuzzle, TReturnPart1, TReturnPart2>
	where TPuzzle : IHappyPuzzle<TReturnPart1, TReturnPart2>, new()
{
	private TPuzzle _sub = new();

	public IEnumerable<Input> ValuesForInput => Helpers.GetAllInputs("Day08");
	[ParamsSource(nameof(ValuesForInput))]
	public required Input Input;

	[Benchmark]
	[BenchmarkCategory("Part 1")]
	public TReturnPart1 Part1()
	{
		return _sub.SolvePart1(Input);
	}

	[Benchmark]
	[BenchmarkCategory("Part 2")]
	public TReturnPart2 Part2()
	{
		return _sub.SolvePart2(Input);
	}
}