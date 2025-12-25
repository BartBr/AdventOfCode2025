using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day11 : HappyPuzzleBase<int, ulong>
{
	// Approach
	// Parse the entire Directed Acyclic Graph (DAG) into 2 Spans
	// The first span is used as a lookup-table where the index is the node id and the value is the index of the slice in the second span.
	// The second span is used to hold the actual references of the original node id, being encoded with the first number indicating the amount of referenced nodes, and subsequently the node ids
	// themselves Which in turn can once again be used to look up in the first span.

	// Use Breath-First Search (BFS) algorithms to traverse the DAG and propagate entry counts through the graph until reaching the target node.
	public override int SolvePart1(Input input)
	{
		//Part1_GenerateGraphVizOutput(input);

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

	// Approach
	// Parse the entire Directed Acyclic Graph (DAG) into 2 Spans
	// The first span is used as a lookup-table where the index is the node id and the value is the index of the slice in the second span.
	// The second span is used to hold the actual references of the original node id, being encoded with the first number indicating the amount of referenced nodes, and subsequently the node ids
	// themselves Which in turn can once again be used to look up in the first span.

	// Contrary to part 1, part 2 apparently has nodes that "jump" down multiple levels, so we first need to analyze the depth of each node in the DAG to be able to correctly count unique paths.
	// In more helpful words, this will actually allow us to decided whether we need to chase the reference right away or whether we can actually wait to a later iteration, as summing right away would
	// result in paths getting accounted for multiple times.

	// Aside from that, the approach is the same as in part 1, except that we now need to calculate for 6 (which can be reduced to 3 if we're lucky) different path combinations
	// 1. svr -> fft
	// 2. fft -> dac
	// 3. dac -> out
	// or
	// 1. svr -> dac
	// 2. dac -> fft
	// 3. fft -> out

	// The latter can be most likely be avoided as we can just calculate the path count of fft -> dac and continue calculating for the first part if it yields a non-zero result.
	// If it does, we need to calculate for the latter part

	// Both the depth analysis and the unique path counting are performed using Breath-First Search (BFS) algorithms.
	public override ulong SolvePart2(Input input)
	{
#if DEBUG
		//Part2_GenerateGraphVizOutput(input);
#endif

		// Each node id consists of three characters, each representing a letter of the alphabet
		// To uniquely identify each node, we can calculate an index based on the letters:
		// index = (firstLetterIndex * 26 * 26) + (secondLetterIndex * 26) + (thirdLetterIndex)
		// Where 'a' = 0, 'b' = 1, ..., 'z' = 25
		// This gives us a range of 0 to 17575 (26^3 - 1)
		scoped Span<int> nodeIndices = stackalloc int[26 * 26 * 26];

		// Assumption that all nodes have on average max 10 references. For my input at least this is very much true with all nodes averaging around 4 references (+ 1 for the reference length variable)
		scoped Span<int> nodeReferenceIndices = stackalloc int[input.Lines.Length * 10];
		var usedNodeReferenceIndices = 0;

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

		const int startNodeId = ('s' - 'a') * 26 * 26 + ('v' - 'a') * 26 + ('r' - 'a'); // "svr" node (root node of directed acyclic graph)
		const int fftNodeId = ('f' - 'a') * 26 * 26 + ('f' - 'a') * 26 + ('t' - 'a'); // "fft" node, intermediate required node
		const int dacNodeId = ('d' - 'a') * 26 * 26 + ('a' - 'a') * 26 + ('c' - 'a'); // "dac" node, intermediate required node
		const int endNodeId = ('o' - 'a') * 26 * 26 + ('u' - 'a') * 26 + ('t' - 'a'); // "out" node (termination node of directed acyclic graph)

		// Analyze depth of each node in the DAG to be able to correctly count unique paths
		scoped Span<int> nodeDepths = stackalloc int[26 * 26 * 26];
		Part2_AnalyzeNodeDepth(nodeIndices, nodeReferenceIndices, startNodeId, ref nodeDepths);

		// Check path count between fft node and dac node (assuming that the order of a path should be start -> fft -> dac -> end)
		// If this yields a result of zero, then those nodes should be traversed in the opposite direction: start -> dac -> fft -> end
		var fftToDacPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, fftNodeId, dacNodeId);
		if (fftToDacPathCount > 0)
		{
			var startToFftPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, startNodeId, fftNodeId);
			var dacToEndPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, dacNodeId, endNodeId);

			return startToFftPathCount * fftToDacPathCount * dacToEndPathCount;
		}

		// This part is supposedly not used (at least not for our input), but leaving this in for correctness.
		var startToDacPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, startNodeId, dacNodeId);
		var dacToFftPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, dacNodeId, fftNodeId);
		var fftToEndPathCount = Part2_CountUniquePaths(nodeIndices, nodeReferenceIndices, nodeDepths, fftNodeId, endNodeId);

		return startToDacPathCount * dacToFftPathCount * fftToEndPathCount;
	}

	// We first analyze the depth at which each node exists. This resolves the issue later on where a node reference jumps > +1 in depth, misleading the unique path counting DFS.
	// Funnily enough, this information also allows us to count all paths more efficiently, as we can just use the depth information to abort the BFS algorithm when it ends of reaching the depth of
	// the target node.
	private static void Part2_AnalyzeNodeDepth(in Span<int> nodeIndices, in Span<int> nodeReferenceIndices, int startNodeId, ref Span<int> nodeDepths)
	{
		const int finalEndNodeId = ('o' - 'a') * 26 * 26 + ('u' - 'a') * 26 + ('t' - 'a'); // "out" node

		// Depth is already at 0 for root node because default initialization of Span<int> is 0, so no need to do a special case for it

		scoped Span<int> currentWaveSearchBuffer = stackalloc int[250];
		currentWaveSearchBuffer[0] = startNodeId;
		var currentWaveSearchBufferLength = 1;

		scoped Span<int> nextWaveSearchBuffer = stackalloc int[250];
		var nextWaveSearchBufferLength = 0;

		// Implement BFS to traverse the subgraph
		// Use nodeId of endNodeId as termination condition

		// This for loop doesn't look correct, as the termination condition doesn't have anything to do with the actual iterations
		// But we're interested in both the iteration count to assign as depth for each node, as well as the termination condition to stop the BFS when there are no more nodes to traverse
		for (var iteration = 1; currentWaveSearchBufferLength > 0; iteration++)
		{
			Debug.WriteLine($"Analyzing nodes at depth {iteration}, currentWaveSearchBufferLength: {currentWaveSearchBufferLength}");

			for (var i = 0; i < currentWaveSearchBufferLength; i++)
			{
				// Get reference nodes for current node
				var currentNodeId = currentWaveSearchBuffer[i];
				if (currentNodeId == finalEndNodeId)
				{
					// Termination condition reached
					continue;
				}

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

					// Set the depth of the reference node to the current iteration
					// If a node was referenced "too early", due to a node jumping down multiple levels of depth, then it will be corrected later on when other nodes reference it at their depth
					nodeDepths[nodeReferenceId] = iteration;
				}
			}

			// Copy next wave buffer to current wave buffer to prepare for next wave/iteration of the BFS
			nextWaveSearchBuffer.Slice(0, nextWaveSearchBufferLength).CopyTo(currentWaveSearchBuffer);
			currentWaveSearchBufferLength = nextWaveSearchBufferLength;
			nextWaveSearchBufferLength = 0;
		}
	}

	private static ulong Part2_CountUniquePaths(in Span<int> nodeIndices, in Span<int> nodeReferenceIndices, in Span<int> nodeDepths, int startNodeId, int endNodeId)
	{
		// Get depth of end node, if we exceed this depth... then there's no point in continue-ing and we better abort the BFS
		var endNodeDepth = nodeDepths[endNodeId];

		scoped Span<uint> nodeEntryCounts = stackalloc uint[26 * 26 * 26];
		nodeEntryCounts[startNodeId] = 1;

		scoped Span<int> currentWaveSearchBuffer = stackalloc int[250];
		currentWaveSearchBuffer[0] = startNodeId;
		var currentWaveSearchBufferLength = 1;

		scoped Span<int> nextWaveSearchBuffer = stackalloc int[250];
		var nextWaveSearchBufferLength = 0;

		// Implement BFS to traverse the subgraph
		// Use a non-matching depth of a node as a termination condition
		// We just need to account for all the nodes that are between the depth of the start and end nodes
		for (var iteration = nodeDepths[startNodeId]; iteration < endNodeDepth; iteration++)
		{
			Debug.WriteLine($"Path counting for depth {iteration}, currentWaveSearchBufferLength: {currentWaveSearchBufferLength}");

			for (var i = 0; i < currentWaveSearchBufferLength; i++)
			{
				// Get reference nodes for current node
				var currentNodeId = currentWaveSearchBuffer[i];
				if (nodeDepths[currentNodeId] != iteration)
				{
					// Skip nodes that are not at the correct depth, they will be traversed later on when the depth is right
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

	[Conditional("DEBUG")]
	private static void Part1_GenerateGraphVizOutput(Input input)
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

	[Conditional("DEBUG")]
	private static void Part2_GenerateGraphVizOutput(Input input)
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

		Console.WriteLine("  srv [style=filled,color=green]");
		Console.WriteLine("  fft [style=filled,color=orange]");
		Console.WriteLine("  dac [style=filled,color=lightblue]");
		Console.WriteLine("  out [style=filled,color=red]");
		Console.WriteLine();

		Console.WriteLine("}");
	}
}