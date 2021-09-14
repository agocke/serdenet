
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Serde.Json
{
    public sealed partial class JsonDeserializer : IDeserializer
    {
        private byte[] _utf8Bytes;
        private JsonReaderState _readerState;
        private int _offset;

        public static JsonDeserializer FromString(string s)
        {
            return new JsonDeserializer(Encoding.UTF8.GetBytes(s));
        }

        private JsonDeserializer(byte[] bytes)
        {
            _utf8Bytes = bytes;
            _readerState = default;
            _offset = 0;
        }

        private void SaveState(in Utf8JsonReader reader)
        {
            _readerState = reader.CurrentState;
            _offset += (int)reader.BytesConsumed;
        }

        private Utf8JsonReader GetReader()
        {
            return new Utf8JsonReader(_utf8Bytes.AsSpan()[_offset..], isFinalBlock: true, _readerState);
        }

        public T DeserializeAny<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            reader.ReadOrThrow();
            T result;
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    result = DeserializeEnumerable(v);
                    break;
                
                case JsonTokenType.Number:
                    result = DeserializeI64(v);
                    break;
                
                default:
                    throw new InvalidDeserializeValueException($"Could not deserialize '{reader.TokenType}");
            }
            return result;
        }

        public T DeserializeBool<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            bool b = reader.GetBoolean();
            SaveState(reader);
            return v.VisitBool(b);
        }

        public T DeserializeByte<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            var b = reader.GetByte();
            SaveState(reader);
            return v.VisitByte(b);
        }

        public T DeserializeDictionary<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeDouble<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            var d = reader.GetDouble();
            _readerState = reader.CurrentState;
            return v.VisitDouble(d);
        }

        public T DeserializeEnumerable<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            reader.ReadOrThrow();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidDeserializeValueException("Expected array start");
            }

            SaveState(reader);
            return v.VisitEnumerable(new DeEnumerable(this));
        }

        private readonly struct DeEnumerable : IDeserializeEnumerable
        {
            private readonly JsonDeserializer _jsDeserializer;
            public DeEnumerable(JsonDeserializer de)
            {
                _jsDeserializer = de;
            }
            public int? SizeOpt => null;

            public bool TryGetNext<T>(IDeserialize<T> deserialize, [MaybeNullWhen(false)] out T next)
            {
                var reader = _jsDeserializer.GetReader();
                // Check if the next token is the end of the array, but don't advance the stream if not
                reader.ReadOrThrow();
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    _jsDeserializer.SaveState(reader);
                    next = default;
                    return false;
                }
                // Don't save state
                next = deserialize.Deserialize(_jsDeserializer);
                return true;
            }
        }

        public T DeserializeFloat<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeI16<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeI32<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeI64<T>(IDeserializeVisitor<T> v)
        {
            var reader = GetReader();
            reader.ReadOrThrow();
            var i64 = reader.GetInt64();
            SaveState(reader);
            return v.VisitI64(i64);
        }

        public T DeserializeSByte<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeString<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeType<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeU16<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeU32<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeU64<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }

        public T DeserializeChar<T>(IDeserializeVisitor<T> v)
        {
            throw new System.NotImplementedException();
        }
    }

    internal static class Utf8JsonReaderExtensions
    {
        public static void ReadOrThrow(ref this Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new InvalidDeserializeValueException("Unexpected end of stream");
            }
        }
    }
}