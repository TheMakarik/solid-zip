using BenchmarkDotNet.Running;
using SolidZip.BenchMarks.Benches;

#if RELEASE
BenchmarkRunner.Run<RangeWhereVsWhere>();
Console.ReadKey();
#elif DEBUG
Console.ForegroundColor = ConsoleColor.Red;
// ReSharper disable once LocalizableElement
Console.WriteLine("Cannot run benchmarks from DEBUG:_, Please run from RELEASE");
Console.ReadKey();
#endif

