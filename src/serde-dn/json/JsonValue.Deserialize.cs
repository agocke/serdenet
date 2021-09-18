
using System.Collections.Immutable;

namespace Serde.Json
{
    internal abstract partial record JsonValue
    {
        public struct DeserializeProxy : IDeserialize<JsonValue>
        {
            JsonValue IDeserialize<JsonValue>.Deserialize(IDeserializer deserializer)
            {
                return deserializer.DeserializeAny(new Visitor());
            }
        }

        private sealed class Visitor : IDeserializeVisitor<JsonValue>
        {
            public string ExpectedTypeName => nameof(JsonValue);

            public JsonValue VisitEnumerable(IDeserializeEnumerable d)
            {
                var builder = ImmutableArray.CreateBuilder<JsonValue>(d.SizeOpt ?? 3);
                while (d.TryGetNext<JsonValue, DeserializeProxy>(default(DeserializeProxy), out var next))
                {
                    builder.Add(next);
                }
                return new Array(builder.ToImmutable());
            }

            public JsonValue VisitI64(long i) => new Number(i);
        }
    }
}