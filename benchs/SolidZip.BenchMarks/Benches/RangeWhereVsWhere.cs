using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SolidZip.Core.Extensions;

namespace SolidZip.BenchMarks.Benches;

[MemoryDiagnoser]
public class RangeWhereVsWhere
{
    private const int EndSearchValue = 56;
    private const int StartSearchValue = 10;
    private readonly int[] _items = Enumerable.Range(0, 100).ToArray();

    [Benchmark]
    public void Where_SimpleSearchWithSearchedOnArrayStart()
    {
        var result = _items.Where(i => i <= EndSearchValue).ToArray();
    }
    
    [Benchmark]
    public void RangeWhere_SimpleSearchWithSearchedOnArrayStart()
    {
        var result = _items.RangeWhere(i => i <= EndSearchValue);
    }
    
    [Benchmark]
    public void Where_SearchWithSearchedOnArrayMiddle()
    {
        var result = _items.Where(i => i is <= EndSearchValue and >= StartSearchValue).ToArray();
    }
    
    [Benchmark]
    public void RangeWhere_SearchWithSearchedOnArrayMiddle()
    {
        var result = _items.RangeWhere(i =>  i is <= EndSearchValue and >= StartSearchValue);
    }
}