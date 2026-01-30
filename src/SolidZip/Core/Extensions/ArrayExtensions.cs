
namespace SolidZip.Core.Extensions;

public static class ArrayExtensions
{
    public static T[] RangeWhere<T>(this T[] array, Predicate<T> predicate)
    {
        var startIndex = (int?)null;
        var endIndex = (int?)null;
        
        for (var i = 0; i < array.Length; i++)
        {
            if (!startIndex.HasValue && predicate(array[i]))
            {
                startIndex = i;
                if (!predicate(array[i + 1]))
                    return [array[i]];
            }
               
            var endPredicateIndex = array.Length - i - 1;
            if (!endIndex.HasValue && predicate(array[endPredicateIndex]))
            {
                endIndex = endPredicateIndex;
                if (!predicate(array[endPredicateIndex - 1]))
                    return [array[endPredicateIndex]];
            }
             
            
            if(startIndex.HasValue && endIndex.HasValue)
                break;
        }
        
        return array[(startIndex.GetValueOrDefault())..(endIndex.GetValueOrDefault() + 1)];
    }
}