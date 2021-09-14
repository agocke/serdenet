
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Serde.Test")]

namespace Serde.Test
{
    internal abstract partial record JsonNode : ISerializeStatic
    {
        private protected JsonNode() { }

        public abstract void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
            where TSerializer : ISerializerStatic<TSerializeType, TSerializeEnumerable, TSerializeDictionary>
            where TSerializeType : ISerializeTypeStatic
            where TSerializeEnumerable : ISerializeEnumerableStatic
            where TSerializeDictionary : ISerializeDictionaryStatic;

        public abstract void Serialize(ISerializer serializer);

        public static implicit operator JsonNode(int i) => new JsonNumber(i);
        public static implicit operator JsonNode(bool b) => new JsonBool(b);
        public static implicit operator JsonNode(string s) => new JsonString(s);
    }
    internal record JsonNumber : JsonNode
    {
        private readonly double _d;
        public JsonNumber(int i) { _d = i; }
        public JsonNumber(long l) { _d = l; }
        public JsonNumber(double d) { _d = d; }

        public override void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
        {
            serializer.SerializeDouble(_d);
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.SerializeDouble(_d);
        }
    }
    internal record JsonBool(bool Value) : JsonNode
    {
        public override void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
        {
            serializer.SerializeBool(Value);
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.SerializeBool(Value);
        }
    }
    internal record JsonString(string Value) : JsonNode
    {
        public override void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
        {
            serializer.SerializeString(Value);
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.SerializeString(Value);
        }
    }
    internal record JsonObject(ImmutableArray<(string FieldName, JsonNode Node)> Members) : JsonNode
    {
        public JsonObject(IEnumerable<(string FieldName, JsonNode Value)> members)
            : this(members.ToImmutableArray())
        { }

        public override void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
        {
            var type = serializer.SerializeType("", Members.Length);
            foreach (var (name, node) in Members)
            {
                type.SerializeField(name, node);
            }
            type.End();
        }

        public override void Serialize(ISerializer serializer)
        {
            var type = serializer.SerializeType("", Members.Length);
            foreach (var (name, node) in Members)
            {
                type.SerializeField(name, node);
            }
            type.End();
        }
    }
    internal sealed record JsonArray(ImmutableArray<JsonNode> Elements) : JsonNode
    {
        public JsonArray(IEnumerable<JsonNode> elements)
            : this(elements.ToImmutableArray())
        { }

        public bool Equals(JsonArray? other)
        {
            if (other is null)
            {
                return false;
            }

            foreach (var (a, b) in Elements.Zip(other.Elements))
            {
                if (!a.Equals(b)) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var e in Elements)
            {
                hash = HashCode.Combine(hash, e);
            }
            return hash;
        }

        public override void Serialize<TSerializer, TSerializeType, TSerializeEnumerable, TSerializeDictionary>(ref TSerializer serializer)
        {
            var enumerable = serializer.SerializeEnumerable(Elements.Length);
            foreach (var element in Elements)
            {
                enumerable.SerializeElement(element);
            }
            enumerable.End();
        }

        public override void Serialize(ISerializer serializer)
        {
            var enumerable = serializer.SerializeEnumerable(Elements.Length);
            foreach (var element in Elements)
            {
                enumerable.SerializeElement(element);
            }
            enumerable.End();
        }
    }
}