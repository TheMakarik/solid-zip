namespace SolidZip.Core.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> enumerable, bool condition, Func<T, bool> filter)
    {
        return condition ? enumerable.Where(filter) : enumerable;
    }
}