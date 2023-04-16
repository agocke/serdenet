﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial class JsonDeserializeTests
    {
        partial record struct IdStructList : Serde.IDeserialize<Serde.Test.JsonDeserializeTests.IdStructList>
        {
            static Serde.Test.JsonDeserializeTests.IdStructList Serde.IDeserialize<Serde.Test.JsonDeserializeTests.IdStructList>.Deserialize<D>(ref D deserializer)
            {
                var visitor = new SerdeVisitor();
                var fieldNames = new[]
                {
                    "Count",
                    "List"
                };
                return deserializer.DeserializeType<Serde.Test.JsonDeserializeTests.IdStructList, SerdeVisitor>("IdStructList", fieldNames, visitor);
            }

            private sealed class SerdeVisitor : Serde.IDeserializeVisitor<Serde.Test.JsonDeserializeTests.IdStructList>
            {
                public string ExpectedTypeName => "Serde.Test.JsonDeserializeTests.IdStructList";
                private struct FieldNameVisitor : Serde.IDeserialize<byte>, Serde.IDeserializeVisitor<byte>
                {
                    public static byte Deserialize<D>(ref D deserializer)
                        where D : IDeserializer => deserializer.DeserializeString<byte, FieldNameVisitor>(new FieldNameVisitor());
                    public string ExpectedTypeName => "string";
                    byte Serde.IDeserializeVisitor<byte>.VisitString(string s) => VisitUtf8Span(System.Text.Encoding.UTF8.GetBytes(s));
                    public byte VisitUtf8Span(System.ReadOnlySpan<byte> s)
                    {
                        switch (s[0])
                        {
                            case (byte)'c'when s.SequenceEqual("count"u8):
                                return 1;
                            case (byte)'l'when s.SequenceEqual("list"u8):
                                return 2;
                            default:
                                return 0;
                        }
                    }
                }

                Serde.Test.JsonDeserializeTests.IdStructList Serde.IDeserializeVisitor<Serde.Test.JsonDeserializeTests.IdStructList>.VisitDictionary<D>(ref D d)
                {
                    Serde.Option<int> count = default;
                    Serde.Option<System.Collections.Generic.List<Serde.Test.JsonDeserializeTests.IdStruct>> list = default;
                    while (d.TryGetNextKey<byte, FieldNameVisitor>(out byte key))
                    {
                        switch (key)
                        {
                            case 1:
                                count = d.GetNextValue<int, Int32Wrap>();
                                break;
                            case 2:
                                list = d.GetNextValue<System.Collections.Generic.List<Serde.Test.JsonDeserializeTests.IdStruct>, ListWrap.DeserializeImpl<Serde.Test.JsonDeserializeTests.IdStruct, Serde.Test.JsonDeserializeTests.IdStruct>>();
                                break;
                        }
                    }

                    var newType = new Serde.Test.JsonDeserializeTests.IdStructList()
                    {
                        Count = count.GetValueOrThrow("Count"),
                        List = list.GetValueOrThrow("List"),
                    };
                    return newType;
                }
            }
        }
    }
}