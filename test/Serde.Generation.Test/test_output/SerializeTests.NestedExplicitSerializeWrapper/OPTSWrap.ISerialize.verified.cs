﻿//HintName: OPTSWrap.ISerialize.cs

#nullable enable
using System;
using Serde;

partial record struct OPTSWrap : Serde.ISerialize<System.Runtime.InteropServices.ComTypes.BIND_OPTS>
{
    void ISerialize<System.Runtime.InteropServices.ComTypes.BIND_OPTS>.Serialize(System.Runtime.InteropServices.ComTypes.BIND_OPTS value, ISerializer serializer)
    {
        var _l_serdeInfo = BIND_OPTSSerdeInfo.Instance;
        var type = serializer.SerializeType(_l_serdeInfo);
        type.SerializeField<int, Int32Wrap>(_l_serdeInfo, 0, value.cbStruct);
        type.SerializeField<int, Int32Wrap>(_l_serdeInfo, 1, value.dwTickCountDeadline);
        type.SerializeField<int, Int32Wrap>(_l_serdeInfo, 2, value.grfFlags);
        type.SerializeField<int, Int32Wrap>(_l_serdeInfo, 3, value.grfMode);
        type.End();
    }
}