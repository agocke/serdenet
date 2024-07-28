
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Serde
{
    /// <summary>
    /// The driving interface for deserializing a given type. This interface should be implemented
    /// for any type that wants to be deserialized by the Serde framework. The implementation should
    /// be independent of the format the type is being deserialized from.
    /// </summary>
    public interface IDeserialize<T> : ISerdeInfoProvider
    {
        abstract static T Deserialize(IDeserializer deserializer);
    }

    /// <summary>
    /// Thrown from implementations of <see cref="IDeserializer" />. Indicates that an unexpected
    /// value was seen in the input which cannot be converted to the target type.
    /// </summary>
    public sealed class InvalidDeserializeValueException : Exception
    {
        public InvalidDeserializeValueException(string msg)
        : base(msg)
        { }
    }

    public interface IDeserializeVisitor<T>
    {
        string ExpectedTypeName { get; }
        T VisitBool(bool b) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitChar(char c) => VisitString(c.ToString());
        T VisitByte(byte b) => VisitU64(b);
        T VisitU16(ushort u16) => VisitU64(u16);
        T VisitU32(uint u32) => VisitU64(u32);
        T VisitU64(ulong u64) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitSByte(sbyte b) => VisitI64(b);
        T VisitI16(short i16) => VisitI64(i16);
        T VisitI32(int i32) => VisitI64(i32);
        T VisitI64(long i64) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitFloat(float f) => VisitDouble(f);
        T VisitDouble(double d) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitDecimal(decimal d) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitString(string s) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitUtf8Span(ReadOnlySpan<byte> s) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitEnumerable<D>(ref D d) where D : IDeserializeEnumerable
            => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitDictionary<D>(ref D d) where D : IDeserializeDictionary
            => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitNull() => throw new InvalidOperationException("Expected type " + ExpectedTypeName);
        T VisitNotNull(IDeserializer d) => throw new InvalidOperationException("Expected type " + ExpectedTypeName);
    }

    public interface IDeserializeCollection
    {
        int? SizeOpt { get; }

        bool TryReadValue<T, D>(ISerdeInfo typeInfo, [MaybeNullWhen(false)] out T next)
            where D : IDeserialize<T>;
    }

    public interface IDeserializeEnumerable
    {
        bool TryGetNext<T, D>([MaybeNullWhen(false)] out T next)
            where D : IDeserialize<T>;
        int? SizeOpt { get; }
    }

    public interface IDeserializeDictionary
    {
        bool TryGetNextKey<K, D>([MaybeNullWhen(false)] out K next)
            where D : IDeserialize<K>;
        V GetNextValue<V, D>() where D : IDeserialize<V>;
        bool TryGetNextEntry<K, DK, V, DV>([MaybeNullWhen(false)] out (K, V) next)
            where DK : IDeserialize<K>
            where DV : IDeserialize<V>;
        int? SizeOpt { get; }
    }

    public interface IDeserializeType
    {
        public const int EndOfType = -1;
        public const int IndexNotFound = -2;

        /// <summary>
        /// Try to read the index of the next field in the type. If the index is found, the method
        /// should return the index and set <paramref name="errorName" /> to null. If the end of the
        /// type is reached, the method should return <see cref="EndOfType" /> and set <paramref
        /// name="errorName" /> to null. If the field is not found, the method should return <see
        /// cref="IndexNotFound" /> and set <paramref name="errorName" /> to the name of the missing
        /// field, or the best-possible user-facing name.
        /// </summary>
        int TryReadIndex(ISerdeInfo map, out string? errorName);

        V ReadValue<V, D>(int index) where D : IDeserialize<V>;
    }

    public interface IDeserializer
    {
        T DeserializeAny<T>(IDeserializeVisitor<T> v);
        T DeserializeBool<T>(IDeserializeVisitor<T> v);
        T DeserializeChar<T>(IDeserializeVisitor<T> v);
        T DeserializeByte<T>(IDeserializeVisitor<T> v);
        T DeserializeU16<T>(IDeserializeVisitor<T> v);
        T DeserializeU32<T>(IDeserializeVisitor<T> v);
        T DeserializeU64<T>(IDeserializeVisitor<T> v);
        T DeserializeSByte<T>(IDeserializeVisitor<T> v);
        T DeserializeI16<T>(IDeserializeVisitor<T> v);
        T DeserializeI32<T>(IDeserializeVisitor<T> v);
        T DeserializeI64<T>(IDeserializeVisitor<T> v);
        T DeserializeFloat<T>(IDeserializeVisitor<T> v);
        T DeserializeDouble<T>(IDeserializeVisitor<T> v);
        T DeserializeDecimal<T>(IDeserializeVisitor<T> v);
        T DeserializeString<T>(IDeserializeVisitor<T> v);
        T DeserializeIdentifier<T>(IDeserializeVisitor<T> v);
        T DeserializeNullableRef<T>(IDeserializeVisitor<T> v);
        IDeserializeCollection DeserializeCollection(ISerdeInfo typeInfo);
        IDeserializeType DeserializeType(ISerdeInfo typeInfo);
    }
}