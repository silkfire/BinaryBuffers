namespace BinaryBuffers.Benchmarks;

using BenchmarkDotNet.Attributes;

using System.IO;

public abstract class BinaryReaderVsBufferReaderBase
{
    protected const int Loops = 5_000_000;

    protected readonly MemoryStream _memoryStream;
    protected readonly BinaryReader _binaryReader;
    protected readonly BinaryBufferReader _bufferReader;

    protected BinaryReaderVsBufferReaderBase()
    {
        var buffer = new byte[1024];
        _memoryStream = new MemoryStream(buffer);
        _binaryReader = new BinaryReader(_memoryStream);
        _bufferReader = new BinaryBufferReader(buffer);
    }
}

public class BinaryReaderVsBufferReader_Int : BinaryReaderVsBufferReaderBase
{
    [Benchmark(Baseline = true)]
    public void BinaryReader_ReadInt()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;

            _binaryReader.ReadInt32();
            _binaryReader.ReadInt64();
        }
    }

    [Benchmark]
    public void BufferReader_ReadInt()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;

            _bufferReader.ReadInt32();
            _bufferReader.ReadInt64();
        }
    }
}

public class BinaryReaderVsBufferReader_Decimal : BinaryReaderVsBufferReaderBase
{
    [Benchmark(Baseline = true)]
    public void BinaryReader_ReadDecimal()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;

            _binaryReader.ReadDecimal();
        }
    }


    [Benchmark]
    public void BufferReader_ReadDecimal()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;

            _bufferReader.ReadDecimal();
        }
    }
}

public class BufferReader_BulkInt32
{
    private const int Loops = 200_000;

    // 128 int32s = one 512-byte sector's worth of SecIds, as read by LiteCDF when building the SAT chain.
    private const int IntCount = 128;

    private readonly BinaryBufferReader _bufferReader;
    private readonly int[] _destination = new int[IntCount];

    public BufferReader_BulkInt32()
    {
        _bufferReader = new BinaryBufferReader(new byte[IntCount * sizeof(int)]);
    }

    [Benchmark(Baseline = true)]
    public void PerElement_ReadInt32_Loop()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;

            for (var j = 0; j < IntCount; j++)
            {
                _destination[j] = _bufferReader.ReadInt32();
            }
        }
    }

    [Benchmark]
    public void Bulk_ReadInto()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;

            _bufferReader.ReadInto<int>(_destination);
        }
    }
}

public class BinaryReaderVsBufferReader_Float : BinaryReaderVsBufferReaderBase
{
    [Benchmark(Baseline = true)]
    public void BinaryReader_ReadFloat()
    {
        for (var i = 0; i < Loops; i++)
        {
            _memoryStream.Position = 0;

            _binaryReader.ReadSingle();
        }
    }


    [Benchmark]
    public void BufferReader_ReadFloat()
    {
        for (var i = 0; i < Loops; i++)
        {
            _bufferReader.Position = 0;

            _bufferReader.ReadSingle();
        }
    }
}
