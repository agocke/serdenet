﻿//HintName: S.ISerialize.cs

#nullable enable
using System;
using Serde;

partial struct S : Serde.ISerialize<S>
{
    void ISerialize<S>.Serialize(S value, ISerializer serializer)
    {
        var _l_serdeInfo = SSerdeInfo.Instance;
        var type = serializer.SerializeType(_l_serdeInfo);
        type.SerializeField<System.Collections.Immutable.ImmutableArray<System.Collections.Specialized.BitVector32.Section>, Serde.ImmutableArrayWrap.SerializeImpl<System.Collections.Specialized.BitVector32.Section, Outer.SectionWrap>>(_l_serdeInfo, 0, value.Sections);
        type.End();
    }
}