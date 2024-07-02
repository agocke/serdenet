﻿//HintName: Some.Nested.Namespace.ColorULongWrap.ISerialize.cs

#nullable enable
using System;
using Serde;

namespace Some.Nested.Namespace
{
    partial struct ColorULongWrap : Serde.ISerialize<Some.Nested.Namespace.ColorULong>
    {
        void ISerialize<Some.Nested.Namespace.ColorULong>.Serialize(Some.Nested.Namespace.ColorULong value, ISerializer serializer)
        {
            var _l_typeInfo = Some.Nested.Namespace.ColorULongSerdeTypeInfo.TypeInfo;
            var index = value switch
            {
                Some.Nested.Namespace.ColorULong.Red => 0,
                Some.Nested.Namespace.ColorULong.Green => 1,
                Some.Nested.Namespace.ColorULong.Blue => 2,
                var v => throw new InvalidOperationException($"Cannot serialize unnamed enum value '{v}' of enum 'ColorULong'"),
            };
            serializer.SerializeEnumValue(_l_typeInfo, index, (ulong)value, default(UInt64Wrap));
        }
    }
}