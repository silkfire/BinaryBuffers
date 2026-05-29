namespace BinaryBuffers.Benchmarks;

using Configurations;

using BenchmarkDotNet.Running;

public static class App
{
    public static void Main()
    {
        BenchmarkRunner.Run(typeof(App).Assembly, new PerformanceConfig());
        // BenchmarkRunner.Run<BinaryReaderVsBufferReader_Decimal>(new PerformanceConfig());
    }
}
