﻿//HintName: A.B.C.D.IDeserialize.cs

#nullable enable
using System;
using Serde;

partial class A
{
    partial class B
    {
        partial class C
        {
            partial class D : Serde.IDeserialize<A.B.C.D>
            {
                static A.B.C.D Serde.IDeserialize<A.B.C.D>.Deserialize(IDeserializer deserializer)
                {
                    var visitor = new SerdeVisitor();
                    var fieldNames = new[]
                    {
                        "Field"
                    };
                    return deserializer.DeserializeType("D", fieldNames, visitor);
                }

                private sealed class SerdeVisitor : Serde.IDeserializeVisitor<A.B.C.D>
                {
                    public string ExpectedTypeName => "A.B.C.D";

                    private sealed class FieldNameVisitor : Serde.IDeserialize<byte>, Serde.IDeserializeVisitor<byte>
                    {
                        public static readonly FieldNameVisitor Instance = new FieldNameVisitor();
                        public static byte Deserialize(IDeserializer deserializer) => deserializer.DeserializeString(Instance);
                        public string ExpectedTypeName => "string";

                        byte Serde.IDeserializeVisitor<byte>.VisitString(string s) => VisitUtf8Span(System.Text.Encoding.UTF8.GetBytes(s));
                        public byte VisitUtf8Span(System.ReadOnlySpan<byte> s)
                        {
                            switch (s[0])
                            {
                                case (byte)'f'when s.SequenceEqual("field"u8):
                                    return 1;
                                default:
                                    return 0;
                            }
                        }
                    }

                    A.B.C.D Serde.IDeserializeVisitor<A.B.C.D>.VisitDictionary<D>(ref D d)
                    {
                        int _l_field = default !;
                        byte _r_assignedValid = 0b0;
                        while (d.TryGetNextKey<byte, FieldNameVisitor>(out byte key))
                        {
                            switch (key)
                            {
                                case 1:
                                    _l_field = d.GetNextValue<int, Int32Wrap>();
                                    _r_assignedValid |= ((byte)1) << 0;
                                    break;
                            }
                        }

                        if (_r_assignedValid != 0b1)
                        {
                            throw new Serde.InvalidDeserializeValueException("Not all members were assigned");
                        }

                        var newType = new A.B.C.D()
                        {
                            Field = _l_field,
                        };
                        return newType;
                    }
                }
            }
        }
    }
}