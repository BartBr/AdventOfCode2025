namespace AdventOfCode2025.Common;

public static class HappyPuzzleHelpers
{
	public static IEnumerable<Type> DiscoverPuzzles(bool onlyLast = false, params string[] users)
	{
		var resolvedPuzzleTypes = typeof(IHappyPuzzle<,>)
			.Assembly
			.GetTypes()
			.Where(x => x is { IsClass: true, IsAbstract: false })
			.Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHappyPuzzle<,>)));

		if (users.Length > 0)
		{
			resolvedPuzzleTypes = resolvedPuzzleTypes
				.Where(x => users.Contains(x.Namespace?.Split('.').Last()));
		}

		var resolvedPuzzleTypesGrouped = resolvedPuzzleTypes
			.GroupBy(x => x.Name[^2..])
			.Select(group => group
				.OrderBy(x => x.Name[^2..])
				.ToList());

		if (onlyLast)
		{
			return resolvedPuzzleTypesGrouped.Last();
		}

		return resolvedPuzzleTypesGrouped.SelectMany(x => x);
	}
}