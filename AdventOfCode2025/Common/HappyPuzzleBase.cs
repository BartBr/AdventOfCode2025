using JetBrains.Annotations;

namespace AdventOfCode2025.Common;

public abstract class HappyPuzzleBase<TReturn> : HappyPuzzleBase<TReturn, TReturn>;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class HappyPuzzleBase<TReturnPart1, TReturnPart2> : IHappyPuzzle<TReturnPart1, TReturnPart2>
{
	public abstract TReturnPart1 SolvePart1(Input input);
	public abstract TReturnPart2 SolvePart2(Input input);
}

public interface IHappyPuzzle<out TReturnPart1, out TReturnPart2>
{
	TReturnPart1 SolvePart1(Input input);
	TReturnPart2 SolvePart2(Input input);
}
