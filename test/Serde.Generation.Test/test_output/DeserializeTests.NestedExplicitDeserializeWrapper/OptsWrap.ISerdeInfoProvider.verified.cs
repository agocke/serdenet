﻿//HintName: OptsWrap.ISerdeInfoProvider.cs

#nullable enable
partial record struct OptsWrap : Serde.ISerdeInfoProvider
{
    static global::Serde.ISerdeInfo global::Serde.ISerdeInfoProvider.SerdeInfo { get; } = Serde.SerdeInfo.MakeCustom(
        "BIND_OPTS",
        new (string, global::Serde.ISerdeInfo, System.Reflection.MemberInfo)[] {
("cbStruct", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(System.Runtime.InteropServices.ComTypes.BIND_OPTS).GetField("cbStruct")!),
("dwTickCountDeadline", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(System.Runtime.InteropServices.ComTypes.BIND_OPTS).GetField("dwTickCountDeadline")!),
("grfFlags", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(System.Runtime.InteropServices.ComTypes.BIND_OPTS).GetField("grfFlags")!),
("grfMode", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(System.Runtime.InteropServices.ComTypes.BIND_OPTS).GetField("grfMode")!)
    });
}