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

Benchmarks show an average of **80%** improvement in reading and **84%** in writing.

Performance tests were executed using **.NET 10** running on a machine with a 16-core CPU.

### BinaryBufferReader

| Method                   | Mean     | Error    | StdDev   | Ratio    |
|------------------------- |---------:|---------:|---------:|---------:|
| `BinaryReader_ReadInt` | 16.633 ms | 0.0406 ms | 0.0380 ms | *baseline* |
| `BufferReader_ReadInt` |  3.607 ms | 0.0020 ms | 0.0019 ms |     -78% |
| `BinaryReader_ReadDecimal` | 14.333 ms | 0.0129 ms | 0.0121 ms | *baseline* |
| `BufferReader_ReadDecimal` | 2.966 ms | 0.0318 ms | 0.0297 ms |     -79% |
| `BinaryReader_ReadFloat` | 11.666 ms | 0.0177 ms | 0.0166 ms | *baseline* |
| `BufferReader_ReadFloat` | 2.012 ms | 0.0011 ms | 0.0011 ms |     -83% |

### BinaryBufferWriter

| Method                    | Mean      | Error     | StdDev    | Ratio    |
|-------------------------- |----------:|----------:|----------:|---------:|
| `BinaryWriter_WriteInt` | 39.373 ms | 0.0547 ms | 0.0511 ms | *baseline* |
| `BufferWriter_WriteInt` | 5.602 ms | 0.0375 ms | 0.0350 ms |     -86% |
| `BinaryWriter_WriteDecimal` | 27.106 ms | 0.0171 ms | 0.0160 ms | *baseline* |
| `BufferWriter_WriteDecimal` |  4.421 ms | 0.0058 ms | 0.0052 ms |     -84% |
| `BinaryWriter_WriteFloat` | 20.406 ms | 0.0205 ms | 0.0192 ms | *baseline* |
| `BufferWriter_WriteFloat` | 3.379 ms | 0.0083 ms | 0.0078 ms |     -83% |
