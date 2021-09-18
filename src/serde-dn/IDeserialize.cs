
using System;
using System.Diagnostics.CodeAnalysis;

namespace Serde
{
    public interface IDeserialize<T>
    {
        T Deserialize(IDeserializer deserializer);
    }

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
        T VisitChar(char c) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitByte(byte b) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitU16(ushort u16) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitU32(uint u32) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitU64(ulong u64) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitSByte(sbyte b) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitI16(short i16) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitI32(int i32) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitI64(long i64) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitFloat(float f) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitDouble(double d) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitString(string s) => throw new InvalidDeserializeValueException("Expected type " + ExpectedTypeName);
        T VisitEnumerable(IDeserializeEnumerable d);
    }

    public interface IDeserializeEnumerable
    {
        bool TryGetNext<T, D>(D deserialize, [MaybeNullWhen(false)] out T next)
            where D : IDeserialize<T>;
        int? SizeOpt { get; }
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
        T DeserializeString<T>(IDeserializeVisitor<T> v);
        T DeserializeType<T>(IDeserializeVisitor<T> v);
        T DeserializeEnumerable<T>(IDeserializeVisitor<T> v);
        T DeserializeDictionary<T>(IDeserializeVisitor<T> v);
    }
}