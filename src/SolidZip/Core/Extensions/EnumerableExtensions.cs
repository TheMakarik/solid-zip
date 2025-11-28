namespace SolidZip.Core.Extensions;

public static class EnumerableExtensions
{
    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> enumerable)
    {
        return new ObservableCollection<T>(enumerable);
    }
}