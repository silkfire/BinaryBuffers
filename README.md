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

writer.Write(2026);
writer.Write(8.11);

// Read from the buffer
var reader = new BinaryBufferReader(buffer);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

# Benchmarks

Benchmarks show an average of **69%** improvement in reading and **79%** in writing.

Performance tests were executed using **.NET 10** running on a machine with a 16-core CPU.

### BinaryBufferReader

| Method                   | Mean     | Error    | StdDev   | Ratio    |
|------------------------- |---------:|---------:|---------:|---------:|
| `BinaryReader_ReadInt` | 16.960 ms | 0.0296 ms | 0.0277 ms | *baseline* |
| `BufferReader_ReadInt` |  5.176 ms | 0.0139 ms | 0.0124 ms |     -69% |
| `BinaryReader_ReadDecimal` | 14.323 ms | 0.253 ms | 0.211 ms | *baseline* |
| `BufferReader_ReadDecimal` | 4.383 ms | 0.0179 ms | 0.104 ms |     -69% |
| `BinaryReader_ReadFloat` | 11.689 ms | 0.0154 ms | 0.0167 ms | *baseline* |
| `BufferReader_ReadFloat` | 3.724 ms | 0.0150 ms | 0.0144 ms |     -68% |

### BinaryBufferWriter

| Method                    | Mean      | Error     | StdDev    | Ratio    |
|-------------------------- |----------:|----------:|----------:|---------:|
| `BinaryWriter_WriteInt` | 39.768 ms | 0.0756 ms | 0.0670 ms | *baseline* |
| `BufferWriter_WriteInt` | 8.361 ms | 0.0120 ms | 0.0107 ms |     -79% |
| `BinaryWriter_WriteDecimal` | 27.283 ms | 0.0133 ms | 0.0118 ms | *baseline* |
| `BufferWriter_WriteDecimal` |  5.725 ms | 0.0078 ms | 0.0328 ms |     -79% |
| `BinaryWriter_WriteFloat` | 20.537 ms | 0.0533 ms | 0.0499 ms | *baseline* |
| `BufferWriter_WriteFloat` | 4.488 ms | 0.0161 ms | 0.0151 ms |     -78% |
