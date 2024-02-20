﻿//HintName: Rgb.IDeserialize.cs

#nullable enable
using System;
using Serde;

partial struct Rgb : Serde.IDeserialize<Rgb>
{
    static Rgb Serde.IDeserialize<Rgb>.Deserialize(IDeserializer deserializer)
    {
        var visitor = new SerdeVisitor();
        var fieldNames = new[]
        {
            "Red",
            "Blue"
        };
        return deserializer.DeserializeType("Rgb", fieldNames, visitor);
    }

    private sealed class SerdeVisitor : Serde.IDeserializeVisitor<Rgb>
    {
        public string ExpectedTypeName => "Rgb";

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
                    case (byte)'r'when s.SequenceEqual("red"u8):
                        return 1;
                    case (byte)'b'when s.SequenceEqual("blue"u8):
                        return 2;
                    default:
                        return 0;
                }
            }
        }

        Rgb Serde.IDeserializeVisitor<Rgb>.VisitDictionary<D>(ref D d)
        {
            byte _l_red = default !;
            byte _l_blue = default !;
            byte _r_assignedValid = 0b0;
            while (d.TryGetNextKey<byte, FieldNameVisitor>(out byte key))
            {
                switch (key)
                {
                    case 1:
                        _l_red = d.GetNextValue<byte, ByteWrap>();
                        _r_assignedValid |= ((byte)1) << 0;
                        break;
                    case 2:
                        _l_blue = d.GetNextValue<byte, ByteWrap>();
                        _r_assignedValid |= ((byte)1) << 1;
                        break;
                }
            }

            if (_r_assignedValid != 0b11)
            {
                throw new Serde.InvalidDeserializeValueException("Not all members were assigned");
            }

            var newType = new Rgb()
            {
                Red = _l_red,
                Blue = _l_blue,
            };
            return newType;
        }
    }
}