﻿//HintName: C.ISerialize.cs

#nullable enable
using System;
using Serde;

partial class C : Serde.ISerialize<C>
{
    void ISerialize<C>.Serialize(C value, ISerializer serializer)
    {
        var _l_serdeInfo = CSerdeInfo.Instance;
        var type = serializer.SerializeType(_l_serdeInfo);
        type.End();
    }
}