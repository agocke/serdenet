﻿//HintName: Some.Nested.Namespace.ColorLongWrap.ISerialize.cs

#nullable enable
using System;
using Serde;

namespace Some.Nested.Namespace
{
    partial struct ColorLongWrap : Serde.ISerialize<Some.Nested.Namespace.ColorLong>
    {
        void ISerialize<Some.Nested.Namespace.ColorLong>.Serialize(Some.Nested.Namespace.ColorLong value, ISerializer serializer)
        {
            var name = value switch
            {
                Some.Nested.Namespace.ColorLong.Red => "red",
                Some.Nested.Namespace.ColorLong.Green => "green",
                Some.Nested.Namespace.ColorLong.Blue => "blue",
                _ => null
            };
            serializer.SerializeEnumValue("ColorLong", name, (long)value, default(Int64Wrap));
        }
    }
}