using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day11 : HappyPuzzleBase<int>
{
	public override int SolvePart1(Input input)
	{
		//GenerateGraphVizOutput(input);

		// Each node id consists of three characters, each representing a letter of the alphabet
		// To uniquely identify each node, we can calculate an index based on the letters:
		// index = (firstLetterIndex * 26 * 26) + (secondLetterIndex * 26) + (thirdLetterIndex)
		// Where 'a' = 0, 'b' = 1, ..., 'z' = 25
		// This gives us a range of 0 to 17575 (26^3 - 1)
		const int startNodeId = ('y' - 'a') * 26 * 26 + ('o' - 'a') * 26 + ('u' - 'a'); // "you" node
		const int endNodeId = ('o' - 'a') * 26 * 26 + ('u' - 'a') * 26 + ('t' - 'a'); // "out" node

		scoped Span<int> nodeIndices = stackalloc int[26 * 26 * 26];

		var usedNodeReferenceIndices = 0;
		scoped Span<int> nodeReferenceIndices = stackalloc int[input.Lines.Length * 10]; // Assumption that all nodes have on average max 10 references

		foreach (var inputLine in input.Lines)
		{
			// Parse Node Id
			// Assumption that node ids only consist of three lowercase letters and are always the first three characters in the line
			var nodeId = (inputLine[0] - 'a') * 26 * 26 + (inputLine[1] - 'a') * 26 + inputLine[2] - 'a';
			nodeIndices[nodeId] = usedNodeReferenceIndices;

			// Skip the first node id, semi-colon and the following whitespace
			var i = 5;
			// Parse all Node Id references
			ref var nodeReferenceLengthIndex = ref nodeReferenceIndices[usedNodeReferenceIndices++];
			for (; i < inputLine.Length; i += 4) // += 4 as we'll be parsing the entire id at once and skip the following whitespace
			{
				var referenceNodeId = (inputLine[i] - 'a') * 26 * 26 + (inputLine[i + 1] - 'a') * 26 + inputLine[i + 2] - 'a';
				nodeReferenceIndices[usedNodeReferenceIndices++] = referenceNodeId;
				nodeReferenceLengthIndex++;
			}
		}

		scoped Span<int> nodeEntryCounts = stackalloc int[26 * 26 * 26];
		nodeEntryCounts[startNodeId] = 1;

		scoped Span<int> currentWaveSearchBuffer = stackalloc int[50];
		currentWaveSearchBuffer[0] = startNodeId;
		var currentWaveSearchBufferLength = 1;

		scoped Span<int> nextWaveSearchBuffer = stackalloc int[50];
		var nextWaveSearchBufferLength = 0;

		// Implement BFS to traverse the subgraph
		// Use nodeId of endNodeId as termination condition
		while (currentWaveSearchBufferLength > 0)
		{
			for (var i = 0; i < currentWaveSearchBufferLength; i++)
			{
				// Get reference nodes for current node
				var currentNodeId = currentWaveSearchBuffer[i];
				if (currentNodeId == endNodeId)
				{
					// Termination condition reached
					continue;
				}

				var currentNodeEntryCount = nodeEntryCounts[currentNodeId];

				var nodeReferenceStartIndex = nodeIndices[currentNodeId];
				var nodeReferenceLength = nodeReferenceIndices[nodeReferenceStartIndex];

				var referenceNodeIndices = nodeReferenceIndices.Slice(nodeReferenceStartIndex + 1, nodeReferenceLength);

				foreach (var nodeReferenceId in referenceNodeIndices)
				{
					// Add reference node to next wave buffer, make sure to check whether it was already traversed in this iteration to prevent duplicate work
					if (!nextWaveSearchBuffer.Slice(0, nextWaveSearchBufferLength).Contains(nodeReferenceId))
					{
						nextWaveSearchBuffer[nextWaveSearchBufferLength++] = nodeReferenceId;
					}

					// Increment entry count for reference node
					nodeEntryCounts[nodeReferenceId] += currentNodeEntryCount;
				}
			}

			// Copy next wave buffer to current wave buffer to prepare for next wave/iteration of the BFS
			nextWaveSearchBuffer.Slice(0, nextWaveSearchBufferLength).CopyTo(currentWaveSearchBuffer);
			currentWaveSearchBufferLength = nextWaveSearchBufferLength;
			nextWaveSearchBufferLength = 0;
		}

		return nodeEntryCounts[endNodeId];
	}

	public override int SolvePart2(Input input)
	{
		throw new NotImplementedException();
	}

	[Conditional("DEBUG")]
	private static void GenerateGraphVizOutput(Input input)
	{
		Console.WriteLine("digraph G {");
		Console.WriteLine();

		foreach (var inputLine in input.Lines)
		{
			var parts = inputLine.Split(':', StringSplitOptions.TrimEntries);
			var start = parts[0];
			var ends = parts[1].Split(' ');
			foreach (var end in ends)
			{
				Console.WriteLine($"  {start} -> {end};");
			}
		}

		Console.WriteLine();

		Console.WriteLine("  you [style=filled,color=green]");
		Console.WriteLine("  out [style=filled,color=red]");
		Console.WriteLine();

		Console.WriteLine("}");
	}
}