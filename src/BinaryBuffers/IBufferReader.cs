namespace BinaryBuffers;

using System;

/// <summary>
/// Represents a reader that can read primitive data types from a binary data source.
/// </summary>
public interface IBufferReader
{
    /// <summary>
    /// Gets the offset into the stream to start reading from.
    /// </summary>
    int Offset { get; }

    /// <summary>
    /// Gets the effective length of the readable region of the stream.
    /// </summary>
    int Length { get; }

    /// <summary>
    /// Gets or sets the current reading position within the stream.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Reads a boolean value from the current binary stream and advances the current position within the stream by one byte.
    /// </summary>
    bool ReadBoolean();

    /// <summary>
    /// Reads the next byte from the current binary stream and advances the current position within the stream by one byte.
    /// </summary>
    byte ReadByte();

    /// <summary>
    /// Reads the specified number of bytes from the current binary stream into a byte array and advances the current position within the stream by that number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    byte[] ReadBytes(int count);

    /// <summary>
    /// Reads a decimal value from the current binary stream and advances the current position within the stream by sixteen bytes.
    /// </summary>
    decimal ReadDecimal();

    /// <summary>
    /// Reads a double-precision floating-point number from the current binary stream and advances the current position within the stream by eight bytes.
    /// </summary>
    double ReadDouble();

    /// <summary>
    /// Reads a 16-bit signed integer from the current binary stream and advances the current position within the stream by two bytes.
    /// </summary>
    short ReadInt16();

    /// <summary>
    /// Reads a 32-bit signed integer from the current binary stream and advances the current position within the stream by four bytes.
    /// </summary>
    int ReadInt32();

    /// <summary>
    /// Reads a 63-bit signed integer from the current binary stream and advances the current position within the stream by eight bytes.
    /// </summary>
    long ReadInt64();

    /// <summary>
    /// Reads a signed byte from the current binary stream and advances the current position within the stream by one byte.
    /// </summary>
    sbyte ReadSByte();

    /// <summary>
    /// Reads single-precision floating-point number from the current binary stream and advances the current position within the stream by four bytes.
    /// </summary>
    float ReadSingle();

    /// <summary>
    /// Reads a span of bytes from the current binary stream and advances the current position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    ReadOnlySpan<byte> ReadSpan(int count);

    /// <summary>
    /// Reads a 16-bit unsigned integer from the current binary stream and advances the current position within the stream by two bytes.
    /// </summary>
    ushort ReadUInt16();

    /// <summary>
    /// Reads a 32-bit unsigned integer from the current binary stream and advances the current position within the stream by four bytes.
    /// </summary>
    uint ReadUInt32();

    /// <summary>
    /// Reads 64-bit unsigned integer from the current binary stream and advances the current position within the stream by eight bytes.
    /// </summary>
    ulong ReadUInt64();

    /// <summary>
    /// Reads an unmanaged value of type <typeparamref name="T"/> from the current binary stream and advances the current position within the stream by the size of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to read. Intended for primitive numeric types; composite layouts such as <see cref="decimal"/> are only read correctly on little-endian platforms (use the dedicated <see cref="ReadDecimal"/> for those).</typeparam>
    T Read<T>() where T : unmanaged;

    /// <summary>
    /// Reads enough unmanaged values of type <typeparamref name="T"/> to fill <paramref name="destination"/> from the current binary stream and advances the current position within the stream by the total number of bytes read.
    /// <para>On little-endian platforms this is a single bulk copy, amortizing away the per-call bounds-checking and endianness branching incurred by repeated <see cref="Read{T}"/> calls in a loop.</para>
    /// </summary>
    /// <typeparam name="T">The unmanaged element type to read. See <see cref="Read{T}"/> for endianness caveats.</typeparam>
    /// <param name="destination">The span to read the values into. Its length determines how many values are read.</param>
    void ReadInto<T>(Span<T> destination) where T : unmanaged;
}
