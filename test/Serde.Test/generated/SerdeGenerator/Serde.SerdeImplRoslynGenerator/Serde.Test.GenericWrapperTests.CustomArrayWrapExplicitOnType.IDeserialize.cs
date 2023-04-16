﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial class GenericWrapperTests
    {
        partial record struct CustomArrayWrapExplicitOnType : Serde.IDeserialize<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>
        {
            static Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType Serde.IDeserialize<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>.Deserialize<D>(ref D deserializer)
            {
                var visitor = new SerdeVisitor();
                var fieldNames = new[]
                {
                    "A"
                };
                return deserializer.DeserializeType<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType, SerdeVisitor>("CustomArrayWrapExplicitOnType", fieldNames, visitor);
            }

            private sealed class SerdeVisitor : Serde.IDeserializeVisitor<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>
            {
                public string ExpectedTypeName => "Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType";
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
                            case (byte)'a'when s.SequenceEqual("a"u8):
                                return 1;
                            default:
                                return 0;
                        }
                    }
                }

                Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType Serde.IDeserializeVisitor<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>.VisitDictionary<D>(ref D d)
                {
                    Serde.Option<Serde.Test.GenericWrapperTests.CustomImArray2<int>> a = default;
                    while (d.TryGetNextKey<byte, FieldNameVisitor>(out byte key))
                    {
                        switch (key)
                        {
                            case 1:
                                a = d.GetNextValue<Serde.Test.GenericWrapperTests.CustomImArray2<int>, Serde.Test.GenericWrapperTests.CustomImArray2Wrap.DeserializeImpl<int, Int32Wrap>>();
                                break;
                        }
                    }

                    var newType = new Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType()
                    {
                        A = a.GetValueOrThrow("A"),
                    };
                    return newType;
                }
            }
        }
    }
}