﻿//HintName: S.ISerdeInfoProvider.cs

#nullable enable
partial struct S : Serde.ISerdeInfoProvider
{
    static global::Serde.ISerdeInfo global::Serde.ISerdeInfoProvider.SerdeInfo { get; } = Serde.SerdeInfo.MakeCustom(
        "S",
        new (string, global::Serde.ISerdeInfo, System.Reflection.MemberInfo)[] {
("one", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(S).GetProperty("One")!),
("twoWord", global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Wrap>(), typeof(S).GetProperty("TwoWord")!)
    });
}