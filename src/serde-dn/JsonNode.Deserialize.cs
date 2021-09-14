
using System.Collections.Immutable;
using Serde.Json;

namespace Serde.Test
{
    internal abstract partial record JsonNode
    {
        public struct DeserializeProxy : IDeserialize<JsonNode>
        {
            JsonNode IDeserialize<JsonNode>.Deserialize(IDeserializer deserializer)
            {
                var visitor = new Visitor(this);
                return deserializer.DeserializeAny(visitor);
            }
        }

        private sealed class Visitor : IDeserializeVisitor<JsonNode>
        {
            private DeserializeProxy _proxy;

            public Visitor(DeserializeProxy proxy)
            {
                _proxy = proxy;
            }
            public string ExpectedTypeName => nameof(JsonNode);

            public JsonNode VisitEnumerable(IDeserializeEnumerable d)
            {
                var builder = ImmutableArray.CreateBuilder<JsonNode>(d.SizeOpt ?? 3);
                while (d.TryGetNext(_proxy, out var next))
                {
                    builder.Add(next);
                }
                return new JsonArray(builder.ToImmutable());
            }

            public JsonNode VisitI64(long i) => new JsonNumber(i);
        }
    }
}