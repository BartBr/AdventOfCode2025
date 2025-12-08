using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day07 : HappyPuzzleBase<int, long>
{
	/// <remarks>
	/// Problem statement involves a partial pascal triangle, and thus will most likely result in a combinatorics exercise.
	/// Guess I need a quick refresher on my math.
	/// </remarks>>
	public override int SolvePart1(Input input)
	{
		var lineWidth = input.Lines[0].Length;
		var startPos = lineWidth / 2;

		Debug.Assert(input.Lines[0][startPos] == 'S');

		scoped Span<int> previousBeams = stackalloc int[lineWidth + 1 / 2];
		previousBeams[0] = startPos;
		var previousBeamsSize = 1;

		scoped Span<int> currentBeams = stackalloc int[previousBeams.Length];
		var currentBeamsSize = 0;

		var tachyonBeamSplitCount = 0;

		// Offset by width to skip first line as no splits can occur there
		for (var i = 2; i < input.Lines.Length; i += 2)
		{
			var lineSpan = input.Lines[i].AsSpan();
			for (var j = 0; j < previousBeamsSize; j++)
			{
				var previousBeamIndex = previousBeams[j];
				// If this is true, that means that a splitter was to the left of the current line index
				// No 2 splitters can ever be adjacent (without an empty space inbetween)
				if (currentBeamsSize > 0 && currentBeams[currentBeamsSize - 1] == previousBeamIndex)
				{
					// NOP
					continue;
				}

				if (lineSpan[previousBeamIndex] == '^')
				{
					++tachyonBeamSplitCount;

					// Same check as above, except if the splitter occured one spot earlier (or there was no splitter at all)
					var leftTachyonBeamIndex = previousBeamIndex - 1;
					if (currentBeamsSize == 0 || currentBeams[currentBeamsSize - 1] != leftTachyonBeamIndex)
					{
						currentBeams[currentBeamsSize++] = leftTachyonBeamIndex;
					}

					currentBeams[currentBeamsSize++] = previousBeamIndex + 1;
				}
				else
				{
					currentBeams[currentBeamsSize++] = previousBeamIndex;
				}
			}

			currentBeams.Slice(0, currentBeamsSize).CopyTo(previousBeams);
			previousBeamsSize = currentBeamsSize;

			currentBeamsSize = 0;
		}

		return tachyonBeamSplitCount;
	}

	public override long SolvePart2(Input input)
	{
		var lineWidth = input.Lines[0].Length;
		var startPos = lineWidth / 2;

		Debug.Assert(input.Lines[0][startPos] == 'S');

		scoped Span<int> previousBeams = stackalloc int[lineWidth + 1 / 2];
		previousBeams[0] = startPos;
		var previousBeamsSize = 1;

		scoped Span<long> previousBeamCombinatorics = stackalloc long[previousBeams.Length];
		previousBeamCombinatorics[0] = 1;

		scoped Span<int> currentBeams = stackalloc int[previousBeams.Length];
		var currentBeamsSize = 0;

		scoped Span<long> currentBeamCombinatorics = stackalloc long[currentBeams.Length];

		// Offset by width to skip first line as no splits can occur there
		for (var i = 2; i < input.Lines.Length; i += 2)
		{
			var lineSpan = input.Lines[i].AsSpan();
			for (var j = 0; j < previousBeamsSize; j++)
			{
				var previousBeamIndex = previousBeams[j];
				var previousBeamCombinations = previousBeamCombinatorics[j];
				// If this is true, that means that a splitter was to the left of the current line index that gets merged with our current beam
				// Therefor we need to combine the possible combinations
				if (currentBeamsSize > 0 && currentBeams[currentBeamsSize - 1] == previousBeamIndex)
				{
					currentBeamCombinatorics[currentBeamsSize - 1] += previousBeamCombinations;
					continue;
				}

				if (lineSpan[previousBeamIndex] == '^')
				{
					var leftTachyonBeamIndex = previousBeamIndex - 1;
					// Check if a beam to the left already exists
					if (currentBeamsSize == 0 || currentBeams[currentBeamsSize - 1] != leftTachyonBeamIndex)
					{
						// It did not, therefor we can just keep on propagating the prior combination count
						currentBeams[currentBeamsSize] = leftTachyonBeamIndex;
						currentBeamCombinatorics[currentBeamsSize] = previousBeamCombinations;
						++currentBeamsSize;
					}
					else
					{
						// It did... now we just need to combine the possible combinations
						currentBeamCombinatorics[currentBeamsSize - 1] += previousBeamCombinations;
					}

					// And add the right beam of split
					currentBeams[currentBeamsSize] = previousBeamIndex + 1;
				}
				else
				{
					// No splitter, just propagate the existing beam to the next line
					currentBeams[currentBeamsSize] = previousBeamIndex;
				}

				// Shared between no splitter and right beam of split
				currentBeamCombinatorics[currentBeamsSize] = previousBeamCombinations;
				++currentBeamsSize;
			}

			// Use our current iteration info as the baseline for next iteration
			currentBeams.Slice(0, currentBeamsSize).CopyTo(previousBeams);
			currentBeamCombinatorics.Slice(0, currentBeamsSize).CopyTo(previousBeamCombinatorics);
			previousBeamsSize = currentBeamsSize;

			currentBeamsSize = 0;
		}

		var tachyonParticleDistinctPathCount = 0L;

		// Add all possible combinations of our latest iteration
		for (var i = 0; i < previousBeamsSize; i++)
		{
			tachyonParticleDistinctPathCount += previousBeamCombinatorics[i];
		}

		return tachyonParticleDistinctPathCount;
	}
}