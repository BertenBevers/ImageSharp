﻿// <copyright file="Rg32.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.PixelFormats
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Packed pixel type containing two 16-bit unsigned normalized values ranging from 0 to 1.
    /// <para>
    /// Ranges from &lt;0, 0, 0, 1&gt; to &lt;1, 1, 0, 1&gt; in vector form.
    /// </para>
    /// </summary>
    public struct Rg32 : IPixel<Rg32>, IPackedVector<uint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rg32"/> struct.
        /// </summary>
        /// <param name="x">The x-component</param>
        /// <param name="y">The y-component</param>
        public Rg32(float x, float y)
        {
            this.PackedValue = Pack(x, y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rg32"/> struct.
        /// </summary>
        /// <param name="vector">The vector containing the component values.</param>
        public Rg32(Vector2 vector)
        {
            this.PackedValue = Pack(vector.X, vector.Y);
        }

        /// <inheritdoc/>
        public uint PackedValue { get; set; }

        /// <summary>
        /// Compares two <see cref="Rg32"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rg32"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rg32"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Rg32 left, Rg32 right)
        {
            return left.PackedValue == right.PackedValue;
        }

        /// <summary>
        /// Compares two <see cref="Rg32"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rg32"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rg32"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rg32 left, Rg32 right)
        {
            return left.PackedValue != right.PackedValue;
        }

        /// <inheritdoc />
        public PixelOperations<Rg32> CreateBulkOperations() => new PixelOperations<Rg32>();

        /// <summary>
        /// Expands the packed representation into a <see cref="Vector2"/>.
        /// The vector components are typically expanded in least to greatest significance order.
        /// </summary>
        /// <returns>The <see cref="Vector2"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ToVector2()
        {
            return new Vector2(
                (this.PackedValue & 0xFFFF) / 65535F,
                ((this.PackedValue >> 16) & 0xFFFF) / 65535F);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PackFromVector4(Vector4 vector)
        {
            this.PackedValue = Pack(vector.X, vector.Y);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 ToVector4()
        {
            return new Vector4(this.ToVector2(), 0F, 1F);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PackFromBytes(byte x, byte y, byte z, byte w)
        {
            this.PackFromVector4(new Vector4(x, y, z, w) / 255F);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToXyzBytes(byte[] bytes, int startIndex)
        {
            Vector4 vector = this.ToVector4() * 255F;

            bytes[startIndex] = (byte)vector.X;
            bytes[startIndex + 1] = (byte)vector.Y;
            bytes[startIndex + 2] = (byte)vector.Z;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToXyzwBytes(byte[] bytes, int startIndex)
        {
            Vector4 vector = this.ToVector4() * 255F;

            bytes[startIndex] = (byte)vector.X;
            bytes[startIndex + 1] = (byte)vector.Y;
            bytes[startIndex + 2] = (byte)vector.Z;
            bytes[startIndex + 3] = (byte)vector.W;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToZyxBytes(byte[] bytes, int startIndex)
        {
            Vector4 vector = this.ToVector4() * 255F;

            bytes[startIndex] = (byte)vector.Z;
            bytes[startIndex + 1] = (byte)vector.Y;
            bytes[startIndex + 2] = (byte)vector.X;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToZyxwBytes(byte[] bytes, int startIndex)
        {
            Vector4 vector = this.ToVector4() * 255F;

            bytes[startIndex] = (byte)vector.Z;
            bytes[startIndex + 1] = (byte)vector.Y;
            bytes[startIndex + 2] = (byte)vector.X;
            bytes[startIndex + 3] = (byte)vector.W;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is Rg32) && this.Equals((Rg32)obj);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Rg32 other)
        {
            return this.PackedValue == other.PackedValue;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToVector2().ToString();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return this.PackedValue.GetHashCode();
        }

        /// <summary>
        /// Packs the <see cref="float"/> components into a <see cref="uint"/>.
        /// </summary>
        /// <param name="x">The x-component</param>
        /// <param name="y">The y-component</param>
        /// <returns>The <see cref="uint"/> containing the packed values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Pack(float x, float y)
        {
            return (uint)(
                ((int)Math.Round(x.Clamp(0, 1) * 65535F) & 0xFFFF) |
                (((int)Math.Round(y.Clamp(0, 1) * 65535F) & 0xFFFF) << 16));
        }
    }
}
