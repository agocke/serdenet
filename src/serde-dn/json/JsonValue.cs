using System.Collections.Immutable;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Serde.Test")]

namespace Serde.Json
{
    internal abstract partial record JsonValue
    {
        private JsonValue() { }

        public static implicit operator JsonValue(int i) => new Number(i);

        public sealed partial record Number(double Value) : JsonValue
        {
            public Number(long l) : this((double)l) { }
        }

        public sealed partial record Bool(bool Value) : JsonValue;
        public sealed partial record String(string Value) : JsonValue;
        public sealed partial record Object(ImmutableArray<(string FieldName, JsonValue Node)> Members) : JsonValue;
        public sealed partial record Array(ImmutableArray<JsonValue> Elements) : JsonValue;
    }
}