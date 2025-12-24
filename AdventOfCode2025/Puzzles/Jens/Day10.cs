using System.Diagnostics;
using System.Numerics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day10 : HappyPuzzleBase<ushort, int>
{
	// Note to self:
	// Part 1 boils down to figuring which buttons need to be XOR-ed together in order to get to the target state.
	// On top of that, we need to figure out the minimum viable XOR-ing operations per line to get to said state, implement using BFS (Breath-First Search)?
	public override ushort SolvePart1(Input input)
	{
		// Assumption that there will be no more than 16 buttons;
		scoped Span<ushort> buttons = stackalloc ushort[16];

		ushort sum = 0;

		foreach (var machineLine in input.Lines)
		{
			var machineLineSpan = machineLine.AsSpan();

			// Parse target state
			// eg: [.##.]
			ushort targetState = 0;

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
				targetState |= (ushort) (1 << (i - 1));
			}

			i += 2;

			// Parse buttons
			// eg: (3) (1,3) (2) (2,3) (0,2) (0,1)
			var currentButtonCount = 0;
			while (machineLineSpan[i] == '(')
			{
				i++;

				ushort currentButtonBitMask = 0;

				for (;; i += 2)
				{
					var c = machineLineSpan[i];
					if (c == ' ')
					{
						break;
					}

					var bitShiftOffset = c - '0';

					currentButtonBitMask |= (ushort) (1 << bitShiftOffset);
				}

				buttons[currentButtonCount++] = currentButtonBitMask;

				i++;
			}

			// No need to parse joltages just yet (according to puwwle explanation)
			var localButtonsSlice = buttons.Slice(0, currentButtonCount);

			// Start algo for current problem statement
			sum += Part1_FindMinimumButtomPressesForBitMask(localButtonsSlice, targetState);
		}

		return sum;
	}

	private static ushort Part1_FindMinimumButtomPressesForBitMask(scoped in Span<ushort> buttons, ushort targetState)
	{
		// Start algo for current problem statement
		ushort depth = 0;
		for (; depth <= buttons.Length; depth++)
		{
			if (Part1_FindMinimumButtomPressesForBitMaskRecursive(buttons, 0, targetState, depth))
			{
				break;
			}
		}

		// Need to +1 because our algorithm is 0-based
		depth++;

		return depth;
	}

	private static bool Part1_FindMinimumButtomPressesForBitMaskRecursive(scoped in Span<ushort> remainingButtons, ushort inputState, ushort targetState, int depth)
	{
		// Reduce how far we look into the remaining buttons by depth
		// Micro optimization to avoid unnecessary (read duplicate) combinations because ABC is same as CBA
		// Furthermore, AAB is the same as B because AA is a No-Op as XOR-ing an even amount with the same bitmask just cancels out the odd amount of XOR-ing with the same bitmask
		var localButtonCount = remainingButtons.Length - depth;
		for (var i = 0; i < localButtonCount; i++)
		{
			var remainingButton = remainingButtons[i];

			// XOR-ing our button (bitmask) into the inputState to invert specified bits
			var currentState = (ushort) (inputState ^ remainingButton);

			if (depth == 0)
			{
				// Reached desired depth, check if we have a match
				if (currentState == targetState)
				{
#if DEBUG
					Debug.Write($"({string.Join(',', Part1_GetButtonDecimals(remainingButton))}) - ");
#endif
					// Match has been found, abort
					return true;
				}
			}
			else
			{
				// Try again with remaining buttons until a depth of 0 has been reached
				if (Part1_FindMinimumButtomPressesForBitMaskRecursive(remainingButtons.Slice(i + 1), currentState, targetState, depth - 1))
				{
#if DEBUG
					Debug.Write($"({string.Join(',', Part1_GetButtonDecimals(remainingButton))}) - ");
#endif
					return true;
				}
			}
		}

		// No targetState match was found for the all passed in remaining buttons and utilized parent buttons
		return false;
	}

#if DEBUG
	private static IEnumerable<uint> Part1_GetButtonDecimals(uint buttonBitMask)
	{
		for (uint i = 0; i < 10; i++)
		{
			var bitMask = 1 << (int) i;

			if ((buttonBitMask & bitMask) == bitMask)
			{
				yield return i;
			}
		}
	}
#endif

	/// Use parity to create a bitmask of the target joltage requirements (eg: {3,5,4,7} -> 1101
	/// That bitmask can be used to determine which buttons needed to be pressed to reach a state where all joltage requirements are even (see part 1)
	/// However... this still requires us to check all possible combinations as there are potentially button combinations that might require more presses upfront to reduce the button pressing later down the chain...
	/// As we will be solving this recusively, we'll have to keep track of which path uses the least amount of button presses to reach the desired state.
	/// Subsequently divide the new even joltage requirements by 2 and do logic we just did over again (recursively),
	/// we know that the minimum required button presses that are determined by the subdivided problem need to multiplied by 2 again though.
	/// Once we hit a 0 joltage requirement across all counters, we know that we have reached the base case and can return the amount of button presses required.
	/// Thus also constantly multipllying the amount of button presses required by 2 per recursion level.
	/// This should yield the minimum amount of button presses required to reach the target joltage requirements
	/// In essence: required button presses = f(relaxed joltage requirements) * 2 + button presses required to get into the relaxed joltage requirements state )
	///	Yippeeeeee
	public override int SolvePart2(Input input)
	{
		// Assumption that there will be no more than 16 buttons
		// Will be sliced to correct size to ease the subsequent algo
		scoped Span<short> buttons = stackalloc short[16];

		scoped Span<short> buttonVectorizationBuffer = stackalloc short[Vector<short>.Count];

		// The same as above, except the bitmask converted to a Vector<short>
		scoped Span<Vector<short>> vectorizedButtons = stackalloc Vector<short>[16];

		// Backing array that keeps track of the target joltage requirements
		// Will be sliced to correct size to ease the subsequent algo
		// Max size is set to Vector<ushort>.Count bc of memory alignment requirements that Vector<T> enforces.
		// On my machine, this is at least 16, but it could technically differ from system to system I assume?
		scoped Span<short> targetJoltageRequirements = stackalloc short[Vector<short>.Count];

		var sum = 0;
		foreach (var machineLine in input.Lines)
		{
			var machineLineSpan = machineLine.AsSpan();

			// Parse joltage requirements
			// eg: {3,5,4,7}
			ushort joltageRequirementCount = 0;
			var joltageRequirementsStartIndex = machineLineSpan.LastIndexOf('{');

			// start is incremented by one to skip the opening '{'
			// end is decremented by one to skip the closing '}'
			short currentNumber = 0;
			for (var i = joltageRequirementsStartIndex + 1; i < machineLineSpan.Length - 1; i++)
			{
				var c = machineLineSpan[i];
				if (c == ',')
				{
					targetJoltageRequirements[joltageRequirementCount++] = currentNumber;
					currentNumber = 0;
				}
				else
				{
					currentNumber = (short) (currentNumber * 10 + (c - '0'));
				}
			}

			targetJoltageRequirements[joltageRequirementCount++] = currentNumber;

			// Clean up the remainder of the joltage requirements buffer to avoid carrying over values from the previous iteration
			targetJoltageRequirements.Slice(joltageRequirementCount).Fill(0);
			// Load the target joltage requirements into a Vector<short>
			var vectorizedTargetJoltageRequirements = new Vector<short>(targetJoltageRequirements);

			// Parse buttons
			// Buttons are loaded in 2 distinct buffers to aid the future processing (and give a shot at vectorization of our problem statement)
			// The first buffer is a simple array of bitmasks, the second buffer is a vectorized array of bitmasks (where all values are either 0 or 1)
			// eg: (3) (1,3) (2) (2,3) (0,2) (0,1)
			var buttonsCount = 0;
			var buttonsStartIndex = machineLineSpan.IndexOf('(');

			for (var i = buttonsStartIndex + 1; i < joltageRequirementsStartIndex - 1; i += 2)
			{
				short currentButtonBitMask = 0;
				// Clean up the buttonVectorizationBuffer to avoid carrying over values from the previous iteration
				buttonVectorizationBuffer.Clear();

				for (;; i += 2)
				{
					var c = machineLineSpan[i];
					if (c == ' ')
					{
						break;
					}

					var bitShiftOffset = c - '0';

					currentButtonBitMask |= (short) (1 << bitShiftOffset);
					buttonVectorizationBuffer[bitShiftOffset] = 1;
				}

				buttons[buttonsCount] = currentButtonBitMask;
				vectorizedButtons[buttonsCount++] = new Vector<short>(buttonVectorizationBuffer);
			}

			// Slice our general buttons span to the correct size
			scoped var buttonsSlice = buttons.Slice(0, buttonsCount);
			// Slice our vectorized buttons span to the correct size
			scoped var vectorizedButtonsSlice = vectorizedButtons.Slice(0, buttonsCount);

#if DEBUG
			foreach (var button in buttonsSlice)
			{
				Debug.Write($"({string.Join(',', Part2_GetButtonDecimals(button))}) - ");
			}
#endif

			// Start the factorization algo for current problem statement
			var part2FindMinimumButtomPressCountRecursive = Part2_FindMinimumButtomPressCountRecursive(buttonsSlice, vectorizedButtonsSlice, vectorizedTargetJoltageRequirements, joltageRequirementCount);

			sum += part2FindMinimumButtomPressCountRecursive;
		}

		return sum;
	}

	// Simple aid to do a vectorized division (hoping it uses SIMD under the hood)
	private static readonly Vector<short> Two = new(2);

	private static int Part2_FindMinimumButtomPressCountRecursive(
		scoped in Span<short> buttons,
		scoped in Span<Vector<short>> vectorizedButtons,
		scoped in Vector<short> inputState,
		in ushort inputStateSize)
	{
		// Check if still valid for factorization (if any of the vector coefficients are negative, then we're taking an invalid path and there's no point in continuing)
		if (Vector.LessThanAny(inputState, Vector<short>.Zero))
		{
			return -1;
		}

		// We've reached a valid combination recursively
		if (Vector.EqualsAll(inputState, Vector<short>.Zero))
		{
			return 0;
		}

		// Calculate the parity bitmask for the current inputState vector (0 if even, 1 if odd)
		ushort parityBitMask = 0;
		// Calculate parity bitmask from inputState
		for (var i = 0; i < inputStateSize; i++)
		{
			if (inputState[i] % 2 == 1)
			{
				parityBitMask |= (ushort) (1 << i);
			}
		}

		// Temporary buffer to be copied into the possibleButtonCombinations span upon degrading the target to exactly 0.
		scoped Span<short> tempSearchBuffer = stackalloc short[buttons.Length];

		// Defining the button combinations buffer width upfront to account for n-buttons + 1 to indicate the length of the combination
		var buttonCombinationsBufferWidth = 1 + buttons.Length;
		// 20 is just an assumption of max possible combinations being found, assuming my previous assumption of <16 buttons also holds true
		const int maxPossibleCombinationCountForBuffer = 20;
		scoped Span<short> possibleButtonCombinations = stackalloc short[buttonCombinationsBufferWidth * maxPossibleCombinationCountForBuffer];
		// Slicing the possibleButtonCombinations span upfront as we will be passing it by-ref and continue slicing it to make it easier working with the indices in the
		// `Part2_FindAllButtomCombinationPressesForBitMask` method
		scoped var possibleButtonCombinationsSlice = possibleButtonCombinations.Slice(0);
		ushort possibleCombinationCount = 0;

		// Because the `Part2_FindAllButtomCombinationPressesForBitMask` doesn't account for not having to press any buttons, we just manually check for that case here.
		// This case only happens if the parityBitMask is fully zero-ed (0), and thus also equals 0 entirely.
		if (parityBitMask == 0)
		{
			// Simply adding 1 to the possibleCombinationCount as we don't need to press any buttons to achieve the desired parityBitMask
			// And thus the length of the combination is also 0, which is the default of the Span values (it's initialized to 0)
			possibleCombinationCount++;
		}

		// Search for all possible button combinations that are applicable for current parityBitMask
		// This method is doing A LOT of duplicate work as we could technically calculate this upfront, but doing this without any allocations is very hard to do >.>
		Part2_FindAllButtomCombinationPressesForBitMask(buttons, parityBitMask, tempSearchBuffer, ref possibleButtonCombinationsSlice, buttonCombinationsBufferWidth, ref possibleCombinationCount);

#if DEBUG
		if (possibleCombinationCount == 0)
		{
			// We shouldn't be here... yet we are...
			return -1;
		}
#endif

		// We keep track of the minimum required button presses for the current parityBitMask here, as we will be iterating over the possibleButtonCombinations span to find the minimum
		var minimumRequiredButtonPresses = -1;

		for (var i = 0; i < possibleCombinationCount; i++)
		{
			// Extracting the required button presses for the current parityBitMask from the possibleButtonCombinations span
			var localIndex = i * buttonCombinationsBufferWidth;
			var minimumRequiredButtonPressesForParityBitMask = possibleButtonCombinations[localIndex];
			var combinationSlice = possibleButtonCombinations.Slice(localIndex + 1, minimumRequiredButtonPressesForParityBitMask);

			// Making a copy of the inputState vector as we will be doing calculations with it over multiple iterations
			var reference = inputState;

			// Subtracting the button presses for the current combination from the reference vector
			foreach (var vectorizedButtonIndex in combinationSlice)
			{
				reference -= vectorizedButtons[vectorizedButtonIndex];
			}

			// Dividing the reference vector by 2, divide and conquer style
			reference /= Two;

			// Recursively calling this method to determine the minimum required button presses for the subdivided problem
			var intermediateButtonPresses = Part2_FindMinimumButtomPressCountRecursive(buttons, vectorizedButtons, reference, inputStateSize);
			if (intermediateButtonPresses == -1)
			{
				// We ended up with either a nega
				continue;
			}

			var requiredButtonPresses = (2 * intermediateButtonPresses) + minimumRequiredButtonPressesForParityBitMask;
			if (minimumRequiredButtonPresses == -1 || requiredButtonPresses < minimumRequiredButtonPresses)
			{
				minimumRequiredButtonPresses = requiredButtonPresses;
			}
		}

		return minimumRequiredButtonPresses;
	}

	// Idea to find all possible button combinations that are able to reduce the targetState to 0
	// Method returns total amount of combinations found that are applicable through it's by-ref variable
	// currentCombination span is used to keep track of the current combination being evaluated, if match found, it's copied to possibleButtonCombinations span
	// possible combinations span is encoded in the following way 1 index available to indicate length of the subsequent following combination, thus length for 1 combination is 1 + total button count
	// That value is then multiplied by 2^button count to account for all possible combinations being valid (this can probably be seriously reduced later on though)
	private static void Part2_FindAllButtomCombinationPressesForBitMask(
		scoped Span<short> buttons,
		ushort targetState,
		scoped Span<short> currentCombination,
		ref Span<short> possibleButtonCombinations,
		in int possibleCombinationsBufferWidth,
		ref ushort possibleCombinationCount,
		scoped in int buttonIndexOffset = 0,
		short depth = 0)
	{
		var localButtonCount = buttons.Length;
		var possibleButtonCombinationLength = (short) (depth + 1);

		for (var i = 0; i < localButtonCount; i++)
		{
			var button = buttons[i];
			currentCombination[depth] = (short) (depth + i + buttonIndexOffset);

			var currentState = (ushort) (targetState ^ button);

			if (currentState == 0)
			{
				possibleButtonCombinations[0] = possibleButtonCombinationLength;
				currentCombination.Slice(0, possibleButtonCombinationLength).CopyTo(possibleButtonCombinations.Slice(1));

				possibleButtonCombinations = possibleButtonCombinations.Slice(possibleCombinationsBufferWidth);
				possibleCombinationCount++;

				#if DEBUG

				#endif
			}

			Part2_FindAllButtomCombinationPressesForBitMask(buttons.Slice(i + 1), currentState, currentCombination, ref possibleButtonCombinations, possibleCombinationsBufferWidth, ref possibleCombinationCount, buttonIndexOffset + i,  possibleButtonCombinationLength);
		}
	}

#if DEBUG
	private static IEnumerable<uint> Part2_GetButtonDecimals(short buttonBitMask)
	{
		for (uint i = 0; i < 10; i++)
		{
			var bitMask = 1 << (int) i;

			if ((buttonBitMask & bitMask) == bitMask)
			{
				yield return i;
			}
		}
	}
#endif
}