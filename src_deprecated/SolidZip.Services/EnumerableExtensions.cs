namespace SolidZip.Services;

public static class EnumerableExtensions
{
    public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, Predicate<T> validation, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            if (validation(item))
                action(item);
            yield return item;
        }
    }
}