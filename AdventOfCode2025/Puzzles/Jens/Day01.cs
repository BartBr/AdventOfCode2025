using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public sealed class Day01 : HappyPuzzleBase<int>
{
	public override int SolvePart1(Input input)
	{
		var password = 0;
		var currentRotation = 50;
		foreach (var lockRotationInstruction in input.Lines)
		{
			var rotationAmount = ParseNumber(lockRotationInstruction.AsSpan(1));
			var rotationDirection = lockRotationInstruction[0];

			if (rotationDirection == 'L')
			{
				rotationAmount *= -1;
			}

			currentRotation += rotationAmount;
			currentRotation %= 100;

			if (currentRotation == 0)
			{
				++password;
			}
		}

		return password;
	}

	public override int SolvePart2(Input input)
	{
		var password = 0;
		var currentRotation = 50;
		foreach (var lockRotationInstruction in input.Lines)
		{
			var rotationAmount = ParseNumber(lockRotationInstruction.AsSpan(1));
			var rotationDirection = lockRotationInstruction[0];

			// Offset for wrap-around
			if (currentRotation == 0 && rotationDirection == 'L')
			{
				currentRotation = 100;
			}
			else if (currentRotation == 100 && rotationDirection == 'R')
			{
				currentRotation = 0;
			}

			var fullRotations = rotationAmount / 100;
			password += fullRotations;
			rotationAmount %= 100;

			if (rotationDirection == 'L')
			{
				rotationAmount *= -1;
			}

			currentRotation += rotationAmount;

			switch (currentRotation)
			{
				case < 0:
					currentRotation += 100;
					++password;
					break;
				case 0:
				case 100:
					++password;
					break;
				case > 100:
					currentRotation -= 100;
					++password;
					break;
			}
		}

		return password;
	}

	private static int ParseNumber(ReadOnlySpan<char> number)
	{
		var result = 0;
		foreach (var ch in number)
		{
			result = result * 10 + (ch - '0');
		}
		return result;
	}
}