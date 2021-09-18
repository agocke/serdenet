
using System;
using System.Text;
using System.Text.Json;

namespace Serde.Json
{
    public sealed class KeyNotStringException : Exception { }

    public sealed partial class JsonSerializer
    {
        /// <summary>
        /// Serialize the given type to a string.
        /// </summary>
        public static string Serialize<T>(T s) where T : ISerialize
        {
            using var bufferWriter = new PooledByteBufferWriter(16 * 1024);
            using var writer = new Utf8JsonWriter(bufferWriter);
            var serializer = new JsonSerializer(writer);
            s.Serialize(serializer);
            writer.Flush();
            return Encoding.UTF8.GetString(bufferWriter.WrittenMemory.Span);
        }

        /// <summary>
        /// Serialize the given type to a string.
        /// </summary>
        public static string SerializeStatic<T>(T s) where T : ISerializeStatic
        {
            using var bufferWriter = new PooledByteBufferWriter(16 * 1024);
            using var writer = new Utf8JsonWriter(bufferWriter);
            var serializer = new JsonSerializerStatic(writer);
            s.Serialize<JsonSerializerStatic, SerializeTypeStatic, SerializeEnumerableStatic, SerializeDictionaryStatic>(ref serializer);
            writer.Flush();
            return Encoding.UTF8.GetString(bufferWriter.WrittenMemory.Span);
        }

        public static T Deserialize<T, TProxy>(string source)
            where TProxy : struct, IDeserialize<T>
        {
            var de = JsonDeserializer.FromString(source);
            return default(TProxy).Deserialize(de);
        }
    }

    // Implementations of ISerializer interfaces
    partial class JsonSerializer : ISerializer
    {
        private JsonSerializerStatic _impl;
        internal JsonSerializer(Utf8JsonWriter writer)
        {
            _impl = new JsonSerializerStatic(writer);
        }

        public ISerializeType SerializeType(string name, int numFields)
        {
            _impl._writer.WriteStartObject();
            return this;
        }

        public ISerializeEnumerable SerializeEnumerable(int? count)
        {
            _impl._writer.WriteStartArray();
            return this;
        }

        public ISerializeDictionary SerializeDictionary(int? count)
        {
            _impl._writer.WriteStartObject();
            return this;
        }

        public void SerializeBool(bool b) => _impl.SerializeBool(b);

        public void SerializeChar(char c) => _impl.SerializeChar(c);

        public void SerializeByte(byte b) => _impl.SerializeByte(b);

        public void SerializeU16(ushort u16) => _impl.SerializeU16(u16);

        public void SerializeU32(uint u32) => _impl.SerializeU32(u32);

        public void SerializeU64(ulong u64) => _impl.SerializeU64(u64);

        public void SerializeSByte(sbyte b) => _impl.SerializeSByte(b);

        public void SerializeI16(short i16) => _impl.SerializeI16(i16);

        public void SerializeI32(int i32) => _impl.SerializeI32(i32);

        public void SerializeI64(long i64) => _impl.SerializeI64(i64);

        public void SerializeFloat(float f) => _impl.SerializeFloat(f);

        public void SerializeDouble(double d) => _impl.SerializeDouble(d);

        public void SerializeString(string s) => _impl.SerializeString(s);
    }

    partial class JsonSerializer : ISerializeType
    {
        void ISerializeType.SerializeField<T>(string name, T value)
        {
            _impl._writer.WritePropertyName(name);
            value.Serialize(this);
        }

        void ISerializeType.End()
        {
            _impl._writer.WriteEndObject();
        }
    }

    partial class JsonSerializer : ISerializeEnumerable
    {
        void ISerializeEnumerable.SerializeElement<T>(T value)
        {
            value.Serialize(this);
        }

        void ISerializeEnumerable.End()
        {
            _impl._writer.WriteEndArray();
        }
    }

    partial class JsonSerializer : ISerializeDictionary
    {
        void ISerializeDictionary.SerializeKey<T>(T key)
        {
            SerializeDictionaryStatic.SerializeKey(ref _impl, key);
        }

        void ISerializeDictionary.SerializeValue<T>(T value)
        {
            value.Serialize(this);
        }

        void ISerializeDictionary.End()
        {
            _impl._writer.WriteEndObject();
        }
    }
}