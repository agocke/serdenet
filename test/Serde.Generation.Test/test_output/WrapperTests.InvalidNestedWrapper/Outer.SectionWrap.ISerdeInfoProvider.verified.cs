﻿//HintName: Outer.SectionWrap.ISerdeInfoProvider.cs

#nullable enable
partial class Outer
{
    partial record struct SectionWrap : Serde.ISerdeInfoProvider
{
    static global::Serde.ISerdeInfo global::Serde.ISerdeInfoProvider.SerdeInfo { get; } = Serde.SerdeInfo.MakeCustom(
        "Section",
        new (string, global::Serde.ISerdeInfo, System.Reflection.MemberInfo)[] {
("mask", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int16Wrap>(), typeof(System.Collections.Specialized.BitVector32.Section).GetProperty("Mask")!),
("offset", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int16Wrap>(), typeof(System.Collections.Specialized.BitVector32.Section).GetProperty("Offset")!)
    });
}
}