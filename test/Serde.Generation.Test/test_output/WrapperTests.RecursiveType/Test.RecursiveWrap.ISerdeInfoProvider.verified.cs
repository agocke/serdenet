﻿//HintName: Test.RecursiveWrap.ISerdeInfoProvider.cs

#nullable enable
namespace Test;
partial record struct RecursiveWrap : Serde.ISerdeInfoProvider
{
    static global::Serde.SerdeInfo global::Serde.ISerdeInfoProvider.SerdeInfo { get; } = Serde.SerdeInfo.Create(
        "Recursive",
        Serde.SerdeInfo.TypeKind.CustomType,
        new (string, global::Serde.SerdeInfo, System.Reflection.MemberInfo)[] {
("next", global::Serde.SerdeInfoProvider.GetInfo<RecursiveWrap>(), typeof(Recursive).GetProperty("Next")!)
    });
}