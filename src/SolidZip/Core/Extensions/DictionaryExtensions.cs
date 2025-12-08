namespace SolidZip.Core.Extensions;

public static class DictionaryExtensions
{
    public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue searchValue) where TValue : IComparable
    {
        return dictionary.First(keyValuePair => keyValuePair.Value.CompareTo(searchValue) == 0).Key;
    }
}