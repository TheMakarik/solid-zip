namespace SolidZip.Core.Common;

public sealed class Result<TResult, TValue>(TResult result, TValue? value = default)
    where TResult : Enum
{
    public TValue? Value { get; } = value;
    
    public bool Is(TResult resultToCompare)
    {
        return EqualityComparer<Enum>.Default.Equals(resultToCompare, result);
    }
}