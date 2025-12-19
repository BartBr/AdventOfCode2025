using System.Diagnostics;
using System.Numerics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day10 : HappyPuzzleBase<int>
{
	// Note to self:
	// Part 1 boils down to figuring which buttons need to be XOR-ed together in order to get to the target state.
	// On top of that, we need to figure out the minimum viable XOR-ing operations per line to get to said state, implement using BFS (Breath-First Search)?
	public override int SolvePart1(Input input)
	{
		// Assumption that there will be no more than 16 buttons;
		scoped Span<uint> buttons = stackalloc uint[16];

		var sum = 0;

		foreach (var machineLine in input.Lines)
		{
			var machineLineSpan = machineLine.AsSpan();

			// Parse target state
			// eg: [.##.]
			uint targetState = 0;

			var i = 1;
			for (; i < machineLineSpan.Length; i++)
			{
				var c = machineLineSpan[i];
				if (c == ']')
				{
					break;
				}

				if (c == '.')
				{
					continue;
				}

				// OR bitshifted bitmask into target state
				// This sets the n-th bit of the target state to 1
				targetState |= (uint) 1 << (i - 1);
			}

			i += 2;

			// Parse buttons
			// eg: (3) (1,3) (2) (2,3) (0,2) (0,1)
			var currentButtonCount = 0;
			while (machineLineSpan[i] == '(')
			{
				i++;

				uint currentButtonBitMask = 0;

				for (;; i += 2)
				{
					var c = machineLineSpan[i];
					if (c == ' ')
					{
						break;
					}

					var bitShiftOffset = c - '0';

					currentButtonBitMask |= (uint) 1 << bitShiftOffset;
				}

				buttons[currentButtonCount++] = currentButtonBitMask;

				i++;
			}

			// No need to parse joltages just yet (according to puwwle explanation)

			// Start algo for current problem statement
			var depth = 0;
			for (; depth <= currentButtonCount; depth++)
			{
				if (FindMinimumButtomPressCountRecursive(buttons.Slice(0, currentButtonCount), 0, targetState, depth))
				{
					break;
				}
			}

			// Need to +1 because our algorithm is 0-based
			depth++;
			sum += depth;

			Debug.WriteLine($"Button press count: {depth} - {machineLine}");
		}

		return sum;
	}

	private static bool FindMinimumButtomPressCountRecursive(scoped in Span<uint> remainingButtons, uint inputState, uint targetState, int depth)
	{
		// Reduce how far we look into the remaining buttons by depth
		// Micro optimization to avoid unnecessary (read duplicate) combinations because ABC is same as CBA
		// Furthermore, AAB is the same as B because AA is a No-Op as XOR-ing an even amount with the same bitmask just cancels out the odd amount of XOR-ing with the same bitmask
		var localButtonCount = remainingButtons.Length - depth;
		for (var i = 0; i < localButtonCount; i++)
		{
			var remainingButton = remainingButtons[i];

			// XOR-ing our button (bitmask) into the inputState to invert specified bits
			var currentState = inputState ^ remainingButton;

			if (depth == 0)
			{
				// Reached desired depth, check if we have a match
				if (currentState == targetState)
				{
#if DEBUG
					Debug.Write($"({string.Join(',', GetButtonDecimals(remainingButton))}) - ");
#endif
					// Match has been found, abort
					return true;
				}
			}
			else
			{
				// Try again with remaining buttons until a depth of 0 has been reached
				if (FindMinimumButtomPressCountRecursive(remainingButtons.Slice(i + 1), currentState, targetState, depth - 1))
				{
#if DEBUG
					Debug.Write($"({string.Join(',', GetButtonDecimals(remainingButton))}) - ");
#endif
					return true;
				}
			}
		}

		// No targetState match was found for the all passed in remaining buttons and utilized parent buttons
		return false;
	}

#if DEBUG
	private static IEnumerable<uint> GetButtonDecimals(uint inputState)
	{
		for (uint i = 0; i < 10; i++)
		{
			var bitMask = 1 << (int) i;

			if ((inputState & bitMask) == bitMask)
			{
				yield return i;
			}
		}
	}
#endif

	public override int SolvePart2(Input input)
	{
		// Assumption that there will be no more than 16 buttons;
		scoped Span<Vector<ushort>> vectorizedButtons = stackalloc Vector<ushort>[16];

		// Intermediate parsing buffer before passed onto Vector<ushort> ctor
		// Stackalloc-ed array should be memory-aligned with the size of a Vector<ushort> (16 indices on my own machine, unsure whether this could vary depending the architecture)
		scoped Span<ushort> vectorParsingBuffer = stackalloc ushort[Vector<ushort>.Count];

		var sum = 0;
		foreach (var machineLine in input.Lines)
		{
			var machineLineSpan = machineLine.AsSpan();

			// Parse joltage requirements
			// eg: {3,5,4,7}
			vectorParsingBuffer.Clear();
			var joltageRequirementCount = 0;
			var joltageRequirementsStartIndex = machineLineSpan.LastIndexOf('{');

			// start is incremented by one to skip the opening '{'
			// end is decremented by one to skip the closing '}'
			ushort currentNumber = 0;
			for (var i = joltageRequirementsStartIndex + 1; i < machineLineSpan.Length - 1; i++)
			{
				var c = machineLineSpan[i];
				if (c == ',')
				{
					vectorParsingBuffer[joltageRequirementCount++] = currentNumber;
					currentNumber = 0;
				}
				else
				{
					currentNumber = (ushort) (currentNumber * 10 + (c - '0'));
				}
			}

			vectorParsingBuffer[joltageRequirementCount] = currentNumber;

			var vectorizedTargetJoltageRequirements = new Vector<ushort>(vectorParsingBuffer);

			// Parse buttons
			// eg: (3) (1,3) (2) (2,3) (0,2) (0,1)
			var buttonsCount = 0;
			var buttonsStartIndex = machineLineSpan.IndexOf('(');

			for (var i = buttonsStartIndex + 1; i < joltageRequirementsStartIndex - 1; i += 2)
			{
				vectorParsingBuffer.Clear();

				for (;; i += 2)
				{
					var c = machineLineSpan[i];
					if (c == ' ')
					{
						break;
					}

					vectorParsingBuffer[c - '0'] = 1;
				}

				vectorizedButtons[buttonsCount++] = new Vector<ushort>(vectorParsingBuffer);
			}

			scoped var vectorizedButtonSlice = vectorizedButtons.Slice(0, buttonsCount);

			// Start algo for current problem statement
			var depth = 0;
			while (!Part2_FindMinimumButtomPressCountRecursive(vectorizedButtonSlice, Vector<ushort>.Zero, vectorizedTargetJoltageRequirements, depth))
			{
				Console.WriteLine($"No result found at depth: {depth}, retrying...");

				depth++;
			}

			// Need to +1 because our algorithm is 0-based
			depth++;
			sum += depth;

			Debug.WriteLine($"Button press count: {depth} - {machineLine}");
			Console.WriteLine("Solved! Current sum is now: " + sum);
		}

		return sum;
	}

	private static bool Part2_FindMinimumButtomPressCountRecursive(scoped in Span<Vector<ushort>> remainingButtons, scoped in Vector<ushort> inputState, scoped in Vector<ushort> targetState, int depth)
	{
		// Contrary to part 1, now we might actually have to press buttons multiple times... *unfortunately*
		var localButtonCount = remainingButtons.Length;
		for (var i = 0; i < localButtonCount; i++)
		{
			var currentButton = remainingButtons[i];

			// XOR-ing our button (bitmask) into the inputState to invert specified bits
			var currentState = inputState + currentButton;

			if (depth == 0)
			{
				// Reached desired depth, check if we have a match
				if (currentState == targetState)
				{
#if DEBUG
					Debug.Write($"({string.Join(',', Part2_GetButtonDecimals(currentButton))}) - ");
#endif
					// Match has been found, abort
					return true;
				}
			}
			else
			{
				// Try again with remaining buttons until a depth of 0 has been reached
				if (Part2_FindMinimumButtomPressCountRecursive(remainingButtons, currentState, targetState, depth - 1))
				{
#if DEBUG
					Debug.Write($"({string.Join(',', Part2_GetButtonDecimals(currentButton))}) - ");
#endif
					return true;
				}
			}
		}

		// No targetState match was found for the all passed in remaining buttons and utilized parent buttons
		return false;
	}

#if DEBUG
	private static IEnumerable<uint> Part2_GetButtonDecimals(Vector<ushort> vectorizedButton)
	{
		for (var i = 0; i < 10; i++)
		{
			if (vectorizedButton[i] == 1)
			{
				yield return (uint) i;
			}
		}
	}
#endif
}