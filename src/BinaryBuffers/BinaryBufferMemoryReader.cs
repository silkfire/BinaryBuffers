namespace BinaryBuffers;

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// Implements an <see cref="IBufferReader"/> that can read primitive data types from a <see cref="byte"/>-based <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="BinaryBufferReader"/> based on the specified <see cref="ReadOnlyMemory{T}"/>.
/// </remarks>
/// <param name="data">The input <see cref="ReadOnlyMemory{T}"/>.</param>
///
public sealed class BinaryBufferMemoryReader(in ReadOnlyMemory<byte> data) : IBufferReader
{
    private readonly ReadOnlyMemory<byte> _data = data;
    private int _position = 0;

    /// <summary>
    /// Gets the offset into the underlying <see cref="ReadOnlyMemory{T}"/> to start reading from.
    /// </summary>
    public int Offset { get; } = 0;

    /// <summary>
    /// Gets the effective length of the readable region of the underlying <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public int Length { get; } = data.Length;

    /// <summary>
    /// Gets or sets the current reading position within the underlying <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public int Position
    {
        get => _position;
        set
        {
            var newPosition = _position + value;

            if (newPosition < 0)
            {
                throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
            }

            if (newPosition > Length)
            {
                throw ExceptionHelper.PositionGreaterThanLengthOfReadOnlyMemoryException(nameof(value));
            }

            _position = newPosition;
        }
    }

    /// <summary>
    /// Reads a boolean value from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
    /// </summary>
    public bool ReadBoolean() => InternalReadByte() != 0;

    /// <summary>
    /// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
    /// </summary>
    public byte ReadByte() => InternalReadByte();

    /// <summary>
    /// Reads the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> into a new byte array and advances the current position by that number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    public byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

    /// <summary>
    /// Reads a decimal value from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by sixteen bytes.
    /// </summary>
    public decimal ReadDecimal()
    {
        ref var source = ref InternalReadRef(16);

        var lo    = Unsafe.ReadUnaligned<int>(ref source);
        var mid   = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref source, 4));
        var hi    = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref source, 8));
        var flags = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref source, 12));

        if (!BitConverter.IsLittleEndian)
        {
            lo    = BinaryPrimitives.ReverseEndianness(lo);
            mid   = BinaryPrimitives.ReverseEndianness(mid);
            hi    = BinaryPrimitives.ReverseEndianness(hi);
            flags = BinaryPrimitives.ReverseEndianness(flags);
        }

        var isNegative = (flags & unchecked((int)0x80000000)) != 0;
        var scale = (byte)((flags >> 16) & 0xFF);

        return new decimal(lo, mid, hi, isNegative, scale);
    }

    /// <summary>
    /// Reads a double-precision floating-point number from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
    /// </summary>
    public double ReadDouble()
    {
        var value = Unsafe.ReadUnaligned<long>(ref InternalReadRef(8));

        if (!BitConverter.IsLittleEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        return BitConverter.Int64BitsToDouble(value);
    }

    /// <summary>
    /// Reads a 16-bit signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by two bytes.
    /// </summary>
    public short ReadInt16()
    {
        var value = Unsafe.ReadUnaligned<short>(ref InternalReadRef(2));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads a 32-bit signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
    /// </summary>
    public int ReadInt32()
    {
        var value = Unsafe.ReadUnaligned<int>(ref InternalReadRef(4));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads a 64-bit signed integer signed integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
    /// </summary>
    public long ReadInt64()
    {
        var value = Unsafe.ReadUnaligned<long>(ref InternalReadRef(8));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads a signed byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
    /// </summary>
    public sbyte ReadSByte() => (sbyte) InternalReadByte();

    /// <summary>
    /// Reads a single-precision floating-point number from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
    /// </summary>
    public float ReadSingle()
    {
        var value = Unsafe.ReadUnaligned<int>(ref InternalReadRef(4));

        if (!BitConverter.IsLittleEndian)
        {
            value = BinaryPrimitives.ReverseEndianness(value);
        }

        return BitConverter.Int32BitsToSingle(value);
    }

    /// <summary>
    /// Reads a span of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by the number of bytes read.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    public ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

    /// <summary>
    /// Reads a 16-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by two bytes.
    /// </summary>
    public ushort ReadUInt16()
    {
        var value = Unsafe.ReadUnaligned<ushort>(ref InternalReadRef(2));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads a 32-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by four bytes.
    /// </summary>
    public uint ReadUInt32()
    {
        var value = Unsafe.ReadUnaligned<uint>(ref InternalReadRef(4));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads a 64-bit unsigned integer from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by eight bytes.
    /// </summary>
    public ulong ReadUInt64()
    {
        var value = Unsafe.ReadUnaligned<ulong>(ref InternalReadRef(8));

        return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
    }

    /// <summary>
    /// Reads the next byte from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by one byte.
    /// </summary>
    private byte InternalReadByte()
    {
        var curPos = _position;
        var newPos = curPos + 1;

        if ((uint)newPos > (uint)Length)
        {
            _position = Length;
            throw ExceptionHelper.EndOfDataException();
        }

        _position = newPos;

        return Unsafe.Add(ref MemoryMarshal.GetReference(_data.Span), (nint)(uint)curPos);
    }

    /// <summary>
    /// Returns a read-only span over the specified number of bytes from the underlying <see cref="ReadOnlyMemory{T}"/> and advances the current position by that number of bytes.
    /// </summary>
    /// <param name="count">The size of the read-only span to return.</param>
    private ReadOnlySpan<byte> InternalReadSpan(int count)
    {
        if (count <= 0)
        {
            return [];
        }

        var curPos = _position;
        var newPos = curPos + count;

        if ((uint)newPos > (uint)Length)
        {
            _position = Length;
            throw ExceptionHelper.EndOfDataException();
        }

        _position = newPos;

        return _data.Span.Slice(curPos, count);
    }

    /// <summary>
    /// Bounds-checks a read of <paramref name="count"/> bytes, advances the current position and returns a reference to the first byte to read from.
    /// </summary>
    /// <param name="count">The number of bytes that will be read.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref byte InternalReadRef(int count)
    {
        var curPos = _position;
        var newPos = curPos + count;

        if ((uint)newPos > (uint)Length)
        {
            _position = Length;
            throw ExceptionHelper.EndOfDataException();
        }

        _position = newPos;

        return ref Unsafe.Add(ref MemoryMarshal.GetReference(_data.Span), (nint)(uint)curPos);
    }
}
