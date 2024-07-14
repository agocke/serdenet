﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial record AllInOne : Serde.ISerialize<Serde.Test.AllInOne>
    {
        void ISerialize<Serde.Test.AllInOne>.Serialize(Serde.Test.AllInOne value, ISerializer serializer)
        {
            var _l_serdeInfo = AllInOneSerdeInfo.Instance;
            var type = serializer.SerializeType(_l_serdeInfo);
            type.SerializeField<bool, BoolWrap>(_l_serdeInfo, 0, value.BoolField);
            type.SerializeField<char, CharWrap>(_l_serdeInfo, 1, value.CharField);
            type.SerializeField<byte, ByteWrap>(_l_serdeInfo, 2, value.ByteField);
            type.SerializeField<ushort, UInt16Wrap>(_l_serdeInfo, 3, value.UShortField);
            type.SerializeField<uint, UInt32Wrap>(_l_serdeInfo, 4, value.UIntField);
            type.SerializeField<ulong, UInt64Wrap>(_l_serdeInfo, 5, value.ULongField);
            type.SerializeField<sbyte, SByteWrap>(_l_serdeInfo, 6, value.SByteField);
            type.SerializeField<short, Int16Wrap>(_l_serdeInfo, 7, value.ShortField);
            type.SerializeField<int, Int32Wrap>(_l_serdeInfo, 8, value.IntField);
            type.SerializeField<long, Int64Wrap>(_l_serdeInfo, 9, value.LongField);
            type.SerializeField<string, StringWrap>(_l_serdeInfo, 10, value.StringField);
            type.SerializeFieldIfNotNull<string?, Serde.NullableRefWrap.SerializeImpl<string, StringWrap>>(_l_serdeInfo, 11, value.NullStringField);
            type.SerializeField<uint[], Serde.ArrayWrap.SerializeImpl<uint, UInt32Wrap>>(_l_serdeInfo, 12, value.UIntArr);
            type.SerializeField<int[][], Serde.ArrayWrap.SerializeImpl<int[], Serde.ArrayWrap.SerializeImpl<int, Int32Wrap>>>(_l_serdeInfo, 13, value.NestedArr);
            type.SerializeField<System.Collections.Immutable.ImmutableArray<int>, Serde.ImmutableArrayWrap.SerializeImpl<int, Int32Wrap>>(_l_serdeInfo, 14, value.IntImm);
            type.SerializeField<Serde.Test.AllInOne.ColorEnum, Serde.Test.AllInOne.ColorEnumWrap>(_l_serdeInfo, 15, value.Color);
            type.End();
        }
    }
}