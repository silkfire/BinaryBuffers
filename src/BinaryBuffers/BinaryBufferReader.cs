namespace BinaryBuffers
{
    using System;
    using System.Buffers.Binary;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implements an <see cref="IBufferReader"/> that can read primitive data types from a byte array.
    /// </summary>
    public sealed class BinaryBufferReader : IBufferReader
    {
        private readonly byte[] _data;
        private int _position;
        private readonly int _endPosition;

        /// <summary>
        /// Gets the offset into the underlying byte array to start reading from.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the effective length of the readable region of the underlying byte array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current reading position within the underlying byte array.
        /// </summary>
        public int Position
        {
            get => _position - Offset;
            set
            {
                if (value < 0)
                {
                    throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
                }

                if (value > Length)
                {
                    throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));
                }

                _position = Offset + value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        public BinaryBufferReader(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _position = 0;
            _endPosition = data.Length;
            Offset = 0;
            Length = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// <para>A provided offset and length specifies the boundaries to use for reading.</para>
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="offset">The 0-based offset into the byte array at which to begin reading from.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        /// <param name="length">The maximum number of bytes that the reader will use for reading, relative to the offset position.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        public BinaryBufferReader(byte[] data, int offset, int length)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));

            if (offset < 0)
            {
                throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
            }

            if (length < 0)
            {
                throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
            }

            if (length > _data.Length - offset)
            {
                throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();
            }

            _position = offset;
            _endPosition = offset + length;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array segment.
        /// </summary>
        /// <param name="data">The byte array segment to read from.</param>
        public BinaryBufferReader(in ArraySegment<byte> data)
        {
            _data = data.Array ?? throw new ArgumentNullException(nameof(data));
            _position = data.Offset;
            _endPosition = data.Offset + data.Count;
            Offset = data.Offset;
            Length = data.Count;
        }



        /// <summary>
        /// Reads a boolean value from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public bool ReadBoolean() => InternalReadByte() != 0;

        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public byte ReadByte() => InternalReadByte();

        /// <summary>
        /// Reads the specified number of bytes from the underlying byte array into a new byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

        /// <summary>
        /// Reads a decimal value from the underlying byte array and advances the current position by sixteen bytes.
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
        /// Reads a double-precision floating-point number from the underlying byte array and advances the current position by eight bytes.
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
        /// Reads a 16-bit signed integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public short ReadInt16()
        {
            var value = Unsafe.ReadUnaligned<short>(ref InternalReadRef(2));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public int ReadInt32()
        {
            var value = Unsafe.ReadUnaligned<int>(ref InternalReadRef(4));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public long ReadInt64()
        {
            var value = Unsafe.ReadUnaligned<long>(ref InternalReadRef(8));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// Reads a signed byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public sbyte ReadSByte() => (sbyte)InternalReadByte();

        /// <summary>
        /// Reads a single-precision floating-point number from the underlying byte array and advances the current position by four bytes.
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
        /// Reads a span of bytes from the underlying byte array and advances the current position by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public ushort ReadUInt16()
        {
            var value = Unsafe.ReadUnaligned<ushort>(ref InternalReadRef(2));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public uint ReadUInt32()
        {
            var value = Unsafe.ReadUnaligned<uint>(ref InternalReadRef(4));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public ulong ReadUInt64()
        {
            var value = Unsafe.ReadUnaligned<ulong>(ref InternalReadRef(8));

            return BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value);
        }


        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        private byte InternalReadByte()
        {
            var pos = _position;
            var newPos = pos + 1;

            if ((uint)newPos > (uint)_endPosition)
            {
                _position = _endPosition;
                throw ExceptionHelper.EndOfDataException();
            }

            _position = newPos;

            return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_data), (nint)(uint)pos);
        }

        /// <summary>
        /// Returns a read-only span over the specified number of bytes from the underlying byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The size of the read-only span to return.</param>
        private ReadOnlySpan<byte> InternalReadSpan(int count)
        {
            if (count <= 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }

            var pos = _position;
            var newPos = pos + count;

            if ((uint)newPos > (uint)_endPosition)
            {
                _position = _endPosition;
                throw ExceptionHelper.EndOfDataException();
            }

            _position = newPos;

            return new ReadOnlySpan<byte>(_data, pos, count);
        }

        /// <summary>
        /// Bounds-checks a read of <paramref name="count"/> bytes, advances the current position and returns a reference to the first byte to read from.
        /// </summary>
        /// <param name="count">The number of bytes that will be read.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref byte InternalReadRef(int count)
        {
            var pos = _position;
            var newPos = pos + count;

            if ((uint)newPos > (uint)_endPosition)
            {
                _position = _endPosition;
                throw ExceptionHelper.EndOfDataException();
            }

            _position = newPos;

            return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_data), (nint)(uint)pos);
        }
    }
}
