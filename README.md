# BinaryBuffers

![logo](https://raw.githubusercontent.com/silkfire/BinaryBuffers/main/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/BinaryBuffers.svg)](https://www.nuget.org/packages/BinaryBuffers)

BinaryBuffers offers a highly performant implementation of `BinaryReader` and `BinaryWriter`, working directly on a `byte` array, thus eliminating the need for an intermediate `Stream` object.

# How to use

`BinaryBufferReader` and `BinaryBufferWriter` are the respective names of the reader and writer. Both classes operate on a `byte[]` as its underlying data buffer.

```csharp
// Provide a buffer to the reader/writer
var buffer = new byte[100];

// Write to the buffer
var writer = new BinaryBufferWriter(buffer);

writer.Write(2019);
writer.Write(8.11);

// Read from the buffer
var reader = new BinaryBufferReader(buffer);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

# Benchmarks

Performance tests were executed using **.NET 8** running on a machine with a 16-core CPU.

### BinaryBufferReader

| Method                   | Mean     | Error    | StdDev   | Ratio    |
|------------------------- |---------:|---------:|---------:|---------:|
| `BinaryReader_ReadInt` | 16.592 ms | 0.0296 ms | 0.0277 ms | *baseline* |
| `BufferReader_ReadInt` |  9.752 ms | 0.0139 ms | 0.0124 ms |     -41% |
| `BinaryReader_ReadDecimal` | 72.75 ms | 0.253 ms | 0.211 ms | *baseline* |
| `BufferReader_ReadDecimal` | 37.22 ms | 0.118 ms | 0.104 ms |     -49% |
| `BinaryReader_ReadFloat` | 9.053 ms | 0.0325 ms | 0.0304 ms | *baseline* |
| `BufferReader_ReadFloat` | 6.257 ms | 0.0150 ms | 0.0133 ms |     -31% |

### BinaryBufferWriter

| Method                    | Mean      | Error     | StdDev    | Ratio    |
|-------------------------- |----------:|----------:|----------:|---------:|
| `BinaryWriter_WriteInt` | 58.33 ms | 0.066 ms | 0.062 ms | *baseline* |
| `BufferWriter_WriteInt` | 21.13 ms | 0.046 ms | 0.043 ms |     -64% |
| `BinaryWriter_WriteDecimal` | 38.592 ms | 0.0729 ms | 0.0569 ms | *baseline* |
| `BufferWriter_WriteDecimal` |  8.847 ms | 0.0351 ms | 0.0328 ms |     -77% |
| `BinaryWriter_WriteFloat` | 30.86 ms | 0.106 ms | 0.100 ms | *baseline* |
| `BufferWriter_WriteFloat` | 10.14 ms | 0.023 ms | 0.020 ms |     -67% |
