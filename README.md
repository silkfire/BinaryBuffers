# BinaryBuffers

![logo](https://raw.githubusercontent.com/silkfire/BinaryBuffers/main/img/logo.png)

[![NuGet](https://img.shields.io/nuget/v/BinaryBuffers.svg)](https://www.nuget.org/packages/BinaryBuffers)

BinaryBuffers offers a highly performant implementation of `BinaryReader` and `BinaryWriter`, working directly on a `byte` array or `ReadOnlyMemory<byte>`, thus eliminating the need for an intermediate `Stream` object.

# How to use

The library provides three types:

- **`BinaryBufferReader`** — reads primitive data types from a `byte[]`.
- **`BinaryBufferWriter`** — writes primitive data types to a `byte[]`.
- **`BinaryBufferMemoryReader`** — reads primitive data types from a `ReadOnlyMemory<byte>`.

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

## Bulk reading

When you need to read a run of values of the same type — e.g. a block of `int`s — `ReadInto<T>` reads them all in a single bounds-checked bulk copy instead of one call per element. On little-endian platforms it compiles down to a single `memcpy`, which is dramatically faster than calling `ReadInt32()` in a loop (see [Benchmarks](#benchmarks)):

```csharp
var reader = new BinaryBufferReader(buffer);

var values = new int[128];
reader.ReadInto<int>(values);   // reads 128 consecutive Int32s in one shot
```

There is also a generic single-value `Read<T>()` for any unmanaged type:

```csharp
var year = reader.Read<int>();
var time = reader.Read<double>();
```

> Both APIs are intended for primitive numeric types. On big-endian platforms each value is byte-swapped, which is correct for integer and floating-point types but not for composite layouts such as `decimal` (use the dedicated `ReadDecimal()` for those).

## Reading from a `ReadOnlyMemory<byte>`

When your data lives in a `ReadOnlyMemory<byte>`, use `BinaryBufferMemoryReader`:

```csharp
ReadOnlyMemory<byte> memory = buffer;

var reader = new BinaryBufferMemoryReader(memory);

var year = reader.ReadInt32();
var time = reader.ReadDouble();
```

## Constructor overloads

`BinaryBufferReader` and `BinaryBufferWriter` can both be constrained to a sub-region of the underlying array by supplying an offset and length:

```csharp
// Read/write within the boundaries [offset, offset + length)
var reader = new BinaryBufferReader(buffer, offset: 10, length: 40);
var writer = new BinaryBufferWriter(buffer, offset: 10, length: 40);
```

`BinaryBufferReader` additionally accepts an `ArraySegment<byte>`, which carries its own offset and count:

```csharp
var segment = new ArraySegment<byte>(buffer, 10, 40);
var reader = new BinaryBufferReader(segment);
```

`BinaryBufferMemoryReader` is constructed from a `ReadOnlyMemory<byte>`; slice the memory beforehand to read from a sub-region:

```csharp
var reader = new BinaryBufferMemoryReader(memory.Slice(10, 40));
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

Reading a block of 128 `Int32`s, bulk `ReadInto<int>` vs. a per-element `ReadInt32()` loop:

| Method                    | Mean       | Ratio    |
|-------------------------- |-----------:|---------:|
| `PerElement_ReadInt32_Loop` | 12,017.4 us | *baseline* |
| `Bulk_ReadInto` |    701.0 us |     -94% |

### BinaryBufferWriter

| Method                    | Mean      | Error     | StdDev    | Ratio    |
|-------------------------- |----------:|----------:|----------:|---------:|
| `BinaryWriter_WriteInt` | 39.373 ms | 0.0547 ms | 0.0511 ms | *baseline* |
| `BufferWriter_WriteInt` | 5.602 ms | 0.0375 ms | 0.0350 ms |     -86% |
| `BinaryWriter_WriteDecimal` | 27.106 ms | 0.0171 ms | 0.0160 ms | *baseline* |
| `BufferWriter_WriteDecimal` |  4.421 ms | 0.0058 ms | 0.0052 ms |     -84% |
| `BinaryWriter_WriteFloat` | 20.406 ms | 0.0205 ms | 0.0192 ms | *baseline* |
| `BufferWriter_WriteFloat` | 3.379 ms | 0.0083 ms | 0.0078 ms |     -83% |
