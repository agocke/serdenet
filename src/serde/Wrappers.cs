// Contains implementations of data interfaces for core types

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Serde
{
    internal sealed record PrimitiveInfo(string TypeName) : ISerdeInfo
    {
        public ISerdeInfo.TypeKind Kind => ISerdeInfo.TypeKind.Primitive;
        public int FieldCount => 0;
        public Utf8Span GetFieldName(int index)
            => throw GetOOR(index);
        public string GetFieldStringName(int index)
            => throw GetOOR(index);
        public IList<System.Reflection.CustomAttributeData> GetFieldAttributes(int index)
            => throw GetOOR(index);

        public int TryGetIndex(Utf8Span fieldName) => IDeserializeType.IndexNotFound;

        public ISerdeInfo GetFieldInfo(int index)
            => throw GetOOR(index);

        private ArgumentOutOfRangeException GetOOR(int index)
            => new ArgumentOutOfRangeException(nameof(index), index, $"{TypeName} has no fields or properties.");
    }

    public readonly struct IdWrap<T> : ISerialize<T>
        where T : ISerialize<T>
    {
        public static ISerdeInfo SerdeInfo => SerdeInfoProvider.GetInfo<T>();
        void ISerialize<T>.Serialize(T value, ISerializer serializer) => value.Serialize(value, serializer);
    }

    public readonly partial record struct BoolWrap
        : ISerialize<bool>, IDeserialize<bool>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);

        private const string s_typeName = "bool";
        void ISerialize<bool>.Serialize(bool value, ISerializer serializer)
            => serializer.SerializeBool(value);
        static bool Serde.IDeserialize<bool>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeBool(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<bool>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            bool IDeserializeVisitor<bool>.VisitBool(bool x) => x;
        }
    }

    public readonly partial record struct CharWrap(char Value)
        : ISerialize<char>, IDeserialize<char>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);

        private const string s_typeName = "char";
        void ISerialize<char>.Serialize(char value, ISerializer serializer)
            => serializer.SerializeChar(value);
        static char Serde.IDeserialize<char>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeChar(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<char>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            char IDeserializeVisitor<char>.VisitChar(char c) => c;
            char IDeserializeVisitor<char>.VisitString(string s) => GetChar(s);
            private char GetChar(string s)
            {
                if (s.Length == 1)
                {
                    return s[0];
                }
                throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
            }
            char IDeserializeVisitor<char>.VisitUtf8Span(ReadOnlySpan<byte> s)
            {
                return GetChar(Encoding.UTF8.GetString(s));
            }
        }
    }

    public readonly partial record struct ByteWrap(byte Value)
        : ISerialize<byte>, IDeserialize<byte>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "byte";
        public void Serialize(byte value, ISerializer serializer)
        {
            serializer.SerializeByte(value);
        }
        static byte Serde.IDeserialize<byte>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeByte(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<byte>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => ByteWrap.s_typeName;
            byte IDeserializeVisitor<byte>.VisitByte(byte b)    => b;
            byte IDeserializeVisitor<byte>.VisitU16(ushort u16) => Convert.ToByte(u16);
            byte IDeserializeVisitor<byte>.VisitU32(uint u32)   => Convert.ToByte(u32);
            byte IDeserializeVisitor<byte>.VisitU64(ulong u64)  => Convert.ToByte(u64);
            byte IDeserializeVisitor<byte>.VisitSByte(sbyte b)  => Convert.ToByte(b);
            byte IDeserializeVisitor<byte>.VisitI16(short i16)  => Convert.ToByte(i16);
            byte IDeserializeVisitor<byte>.VisitI32(int i32)    => Convert.ToByte(i32);
            byte IDeserializeVisitor<byte>.VisitI64(long i64)   => Convert.ToByte(i64);
        }
    }

    public readonly partial record struct UInt16Wrap(ushort Value)
        : ISerialize<ushort>, IDeserialize<ushort>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "ushort";
        void ISerialize<ushort>.Serialize(ushort value, ISerializer serializer)
        {
            serializer.SerializeU16(value);
        }
        static ushort Serde.IDeserialize<ushort>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeU16(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<ushort>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            ushort IDeserializeVisitor<ushort>.VisitByte(byte b)    => b;
            ushort IDeserializeVisitor<ushort>.VisitU16(ushort u16) => u16;
            ushort IDeserializeVisitor<ushort>.VisitU32(uint u32)   => Convert.ToUInt16(u32);
            ushort IDeserializeVisitor<ushort>.VisitU64(ulong u64)  => Convert.ToUInt16(u64);
            ushort IDeserializeVisitor<ushort>.VisitSByte(sbyte b)  => Convert.ToUInt16(b);
            ushort IDeserializeVisitor<ushort>.VisitI16(short i16)  => Convert.ToUInt16(i16);
            ushort IDeserializeVisitor<ushort>.VisitI32(int i32)    => Convert.ToUInt16(i32);
            ushort IDeserializeVisitor<ushort>.VisitI64(long i64)   => Convert.ToUInt16(i64);
        }
    }

    public readonly partial record struct UInt32Wrap
        : ISerialize<uint>, IDeserialize<uint>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "uint";
        void ISerialize<uint>.Serialize(uint value, ISerializer serializer)
            => serializer.SerializeU32(value);
        static uint Serde.IDeserialize<uint>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeU32(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<uint>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            uint IDeserializeVisitor<uint>.VisitByte(byte b)    => b;
            uint IDeserializeVisitor<uint>.VisitU16(ushort u16) => u16;
            uint IDeserializeVisitor<uint>.VisitU32(uint u32)   => u32;
            uint IDeserializeVisitor<uint>.VisitU64(ulong u64)  => Convert.ToUInt32(u64);
            uint IDeserializeVisitor<uint>.VisitSByte(sbyte b)  => Convert.ToUInt32(b);
            uint IDeserializeVisitor<uint>.VisitI16(short i16)  => Convert.ToUInt32(i16);
            uint IDeserializeVisitor<uint>.VisitI32(int i32)    => Convert.ToUInt32(i32);
            uint IDeserializeVisitor<uint>.VisitI64(long i64)   => Convert.ToUInt32(i64);
        }
    }

    public readonly partial record struct UInt64Wrap(ulong Value)
        : ISerialize<ulong>, IDeserialize<ulong>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "ulong";
        void ISerialize<ulong>.Serialize(ulong value, ISerializer serializer)
            => serializer.SerializeU64(value);
        static ulong Serde.IDeserialize<ulong>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeU64(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<ulong>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            ulong IDeserializeVisitor<ulong>.VisitByte(byte b)    => b;
            ulong IDeserializeVisitor<ulong>.VisitU16(ushort u16) => u16;
            ulong IDeserializeVisitor<ulong>.VisitU32(uint u32)   => u32;
            ulong IDeserializeVisitor<ulong>.VisitU64(ulong u64)  => u64;
            ulong IDeserializeVisitor<ulong>.VisitSByte(sbyte b)  => Convert.ToUInt64(b);
            ulong IDeserializeVisitor<ulong>.VisitI16(short i16)  => Convert.ToUInt64(i16);
            ulong IDeserializeVisitor<ulong>.VisitI32(int i32)    => Convert.ToUInt64(i32);
            ulong IDeserializeVisitor<ulong>.VisitI64(long i64)   => Convert.ToUInt64(i64);
        }
    }

    public readonly partial record struct SByteWrap(sbyte Value)
        : ISerialize<sbyte>, IDeserialize<sbyte>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "sbyte";
        void ISerialize<sbyte>.Serialize(sbyte value, ISerializer serializer)
            => serializer.SerializeSByte(value);
        static sbyte Serde.IDeserialize<sbyte>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeSByte(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<sbyte>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            sbyte IDeserializeVisitor<sbyte>.VisitByte(byte b)    => Convert.ToSByte(b);
            sbyte IDeserializeVisitor<sbyte>.VisitU16(ushort u16) => Convert.ToSByte(u16);
            sbyte IDeserializeVisitor<sbyte>.VisitU32(uint u32)   => Convert.ToSByte(u32);
            sbyte IDeserializeVisitor<sbyte>.VisitU64(ulong u64)  => Convert.ToSByte(u64);
            sbyte IDeserializeVisitor<sbyte>.VisitSByte(sbyte b)  => b;
            sbyte IDeserializeVisitor<sbyte>.VisitI16(short i16)  => Convert.ToSByte(i16);
            sbyte IDeserializeVisitor<sbyte>.VisitI32(int i32)    => Convert.ToSByte(i32);
            sbyte IDeserializeVisitor<sbyte>.VisitI64(long i64)   => Convert.ToSByte(i64);
        }
    }

    public readonly partial record struct Int16Wrap(short Value)
        : ISerialize<short>, IDeserialize<short>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "short";
        void ISerialize<short>.Serialize(short value, ISerializer serializer)
        {
            serializer.SerializeI16(value);
        }
        static short Serde.IDeserialize<short>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeI16(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<short>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            short IDeserializeVisitor<short>.VisitByte(byte b)    => b;
            short IDeserializeVisitor<short>.VisitU16(ushort u16) => Convert.ToInt16(u16);
            short IDeserializeVisitor<short>.VisitU32(uint u32)   => Convert.ToInt16(u32);
            short IDeserializeVisitor<short>.VisitU64(ulong u64)  => Convert.ToInt16(u64);
            short IDeserializeVisitor<short>.VisitSByte(sbyte b)  => b;
            short IDeserializeVisitor<short>.VisitI16(short i16)  => i16;
            short IDeserializeVisitor<short>.VisitI32(int i32)    => Convert.ToInt16(i32);
            short IDeserializeVisitor<short>.VisitI64(long i64)   => Convert.ToInt16(i64);
        }
    }

    public readonly partial record struct Int32Wrap(int Value)
        : ISerialize<int>, IDeserialize<int>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "int";
        void ISerialize<int>.Serialize(int value, ISerializer serializer)
        {
            serializer.SerializeI32(value);
        }
        static int Serde.IDeserialize<int>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeI32(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<int>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            int IDeserializeVisitor<int>.VisitByte(byte b)    => b;
            int IDeserializeVisitor<int>.VisitU16(ushort u16) => u16;
            int IDeserializeVisitor<int>.VisitU32(uint u32)   => Convert.ToInt32(u32);
            int IDeserializeVisitor<int>.VisitU64(ulong u64)  => Convert.ToInt32(u64);
            int IDeserializeVisitor<int>.VisitSByte(sbyte b)  => b;
            int IDeserializeVisitor<int>.VisitI16(short i16)  => i16;
            int IDeserializeVisitor<int>.VisitI32(int i32)    => i32;
            int IDeserializeVisitor<int>.VisitI64(long i64)   => Convert.ToInt32(i64);
        }
    }

    public readonly partial record struct Int64Wrap(long Value)
        : ISerialize<long>, IDeserialize<long>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "long";
        void ISerialize<long>.Serialize(long value, ISerializer serializer)
            => serializer.SerializeI64(value);
        static long Serde.IDeserialize<long>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeI64(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<long>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            long IDeserializeVisitor<long>.VisitByte(byte b)    => b;
            long IDeserializeVisitor<long>.VisitU16(ushort u16) => u16;
            long IDeserializeVisitor<long>.VisitU32(uint u32)   => u32;
            long IDeserializeVisitor<long>.VisitU64(ulong u64)  => Convert.ToInt64(u64);
            long IDeserializeVisitor<long>.VisitSByte(sbyte b)  => b;
            long IDeserializeVisitor<long>.VisitI16(short i16)  => i16;
            long IDeserializeVisitor<long>.VisitI32(int i32)    => i32;
            long IDeserializeVisitor<long>.VisitI64(long i64)   => Convert.ToInt64(i64);
        }
    }

    public readonly partial record struct DoubleWrap(double Value)
        : ISerialize<double>, IDeserialize<double>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo("double");
        public static DoubleWrap Create(double d) => new DoubleWrap(d);
        void ISerialize<double>.Serialize(double value, ISerializer serializer)
        {
            serializer.SerializeDouble(value);
        }
        static double Serde.IDeserialize<double>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeDouble(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<double>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => "double";
            double IDeserializeVisitor<double>.VisitByte(byte b)    => b;
            double IDeserializeVisitor<double>.VisitU16(ushort u16) => u16;
            double IDeserializeVisitor<double>.VisitU32(uint u32)   => u32;
            double IDeserializeVisitor<double>.VisitU64(ulong u64)  => Convert.ToDouble(u64);
            double IDeserializeVisitor<double>.VisitSByte(sbyte b)  => b;
            double IDeserializeVisitor<double>.VisitI16(short i16)  => i16;
            double IDeserializeVisitor<double>.VisitI32(int i32)    => i32;
            double IDeserializeVisitor<double>.VisitI64(long i64)   => Convert.ToDouble(i64);
            double IDeserializeVisitor<double>.VisitFloat(float f) => f;
            double IDeserializeVisitor<double>.VisitDouble(double d) => d;
        }
    }

    public readonly partial record struct DecimalWrap(decimal Value)
        : ISerialize<decimal>, IDeserialize<decimal>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo("decimal");
        public static DecimalWrap Create(decimal d) => new DecimalWrap(d);
        void Serde.ISerialize<decimal>.Serialize(decimal value, ISerializer serializer)
        {
            serializer.SerializeDecimal(value);
        }
        static decimal Serde.IDeserialize<decimal>.Deserialize(IDeserializer deserializer)
        {
            var visitor = SerdeVisitor.Instance;
            return deserializer.DeserializeDecimal(visitor);
        }

        private sealed class SerdeVisitor : Serde.IDeserializeVisitor<decimal>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => "decimal";
            decimal IDeserializeVisitor<decimal>.VisitByte(byte b)    => b;
            decimal IDeserializeVisitor<decimal>.VisitU16(ushort u16) => u16;
            decimal IDeserializeVisitor<decimal>.VisitU32(uint u32)   => u32;
            decimal IDeserializeVisitor<decimal>.VisitU64(ulong u64)  => u64;
            decimal IDeserializeVisitor<decimal>.VisitSByte(sbyte b)  => b;
            decimal IDeserializeVisitor<decimal>.VisitI16(short i16)  => i16;
            decimal IDeserializeVisitor<decimal>.VisitI32(int i32)    => i32;
            decimal IDeserializeVisitor<decimal>.VisitI64(long i64)   => i64;
            decimal IDeserializeVisitor<decimal>.VisitFloat(float f) => Convert.ToDecimal(f);
            decimal IDeserializeVisitor<decimal>.VisitDouble(double d) => Convert.ToDecimal(d);
            decimal IDeserializeVisitor<decimal>.VisitDecimal(decimal d) => d;
        }
    }
    public readonly partial record struct StringWrap(string Value)
        : ISerialize<string>, IDeserialize<string>
    {
        public static ISerdeInfo SerdeInfo { get; } = new PrimitiveInfo(s_typeName);
        private const string s_typeName = "string";
        public static StringWrap Create(string s) => new StringWrap(s);
        void ISerialize<string>.Serialize(string value, ISerializer serializer)
        {
            serializer.SerializeString(value);
        }
        public static string Deserialize(IDeserializer deserializer)
        {
            return deserializer.DeserializeString(SerdeVisitor.Instance);
        }

        private sealed class SerdeVisitor : IDeserializeVisitor<string>
        {
            public static readonly SerdeVisitor Instance = new SerdeVisitor();
            public string ExpectedTypeName => s_typeName;
            public string VisitString(string s) => s;
            string IDeserializeVisitor<string>.VisitChar(char c) => c.ToString();
            string IDeserializeVisitor<string>.VisitUtf8Span(ReadOnlySpan<byte> s) => Encoding.UTF8.GetString(s);
        }
    }

    public static class NullableWrap
    {
        internal class SerdeInfo<T> where T : ISerdeInfoProvider
        {
            public static readonly ISerdeInfo Instance = new WrapperSerdeInfo(T.SerdeInfo.TypeName + "?", T.SerdeInfo);
        }

        public readonly partial record struct SerializeImpl<T, TWrap> : ISerialize<T?>
            where T : struct
            where TWrap : struct, ISerialize<T>
        {
            public static ISerdeInfo SerdeInfo => SerdeInfo<TWrap>.Instance;
            void ISerialize<T?>.Serialize(T? value, ISerializer serializer)
            {
                if (value is {} notnull)
                {
                    default(TWrap).Serialize(notnull, serializer);
                }
                else
                {
                    serializer.SerializeNull();
                }
            }
        }

        public readonly partial record struct DeserializeImpl<T, TWrap>
            : IDeserialize<T?>
            where T : struct
            where TWrap : IDeserialize<T>
        {
            public static ISerdeInfo SerdeInfo => SerdeInfo<TWrap>.Instance;
            public static T? Deserialize(IDeserializer deserializer)
            {
                return deserializer.DeserializeNullableRef(new Visitor());
            }

            private sealed class Visitor : IDeserializeVisitor<T?>
            {
                public string ExpectedTypeName => typeof(T).ToString() + "?";

                T? IDeserializeVisitor<T?>.VisitNull()
                {
                    return null;
                }

                T? IDeserializeVisitor<T?>.VisitNotNull(IDeserializer d)
                {
                    return TWrap.Deserialize(d);
                }
            }
        }
    }

    public static class NullableRefWrap
    {
        public readonly partial record struct SerializeImpl<T, TWrap> : ISerialize<T?>
            where T : class
            where TWrap : struct, ISerialize<T>
        {
            public static ISerdeInfo SerdeInfo => NullableWrap.SerdeInfo<TWrap>.Instance;

            void ISerialize<T?>.Serialize(T? value, ISerializer serializer)
            {
                if (value is null)
                {
                    serializer.SerializeNull();
                }
                else
                {
                    default(TWrap).Serialize(value, serializer);
                }
            }
        }

        public readonly partial record struct DeserializeImpl<T, TWrap>(T? Value)
            : IDeserialize<T?>
            where T : class
            where TWrap : IDeserialize<T>
        {
            public static ISerdeInfo SerdeInfo => NullableWrap.SerdeInfo<TWrap>.Instance;

            public static T? Deserialize(IDeserializer deserializer)
            {
                return deserializer.DeserializeNullableRef(new Visitor());
            }

            private sealed class Visitor : IDeserializeVisitor<T?>
            {
                public string ExpectedTypeName => typeof(T).ToString() + "?";

                T? IDeserializeVisitor<T?>.VisitNull()
                {
                    return null;
                }

                T? IDeserializeVisitor<T?>.VisitNotNull(IDeserializer d)
                {
                    return TWrap.Deserialize(d);
                }
            }
        }
    }
}