﻿//HintName: RgbProxy.ISerdeInfoProvider.cs

#nullable enable
partial class RgbProxy : Serde.ISerdeInfoProvider
{
    static global::Serde.ISerdeInfo global::Serde.ISerdeInfoProvider.SerdeInfo { get; } = Serde.SerdeInfo.MakeEnum(
        "Rgb",
        typeof(Rgb).GetCustomAttributesData(),
        global::Serde.SerdeInfoProvider.GetInfo<global::Serde.Int32Proxy>(),
        new (string, System.Reflection.MemberInfo)[] {
            ("red", typeof(Rgb).GetField("Red")!),
            ("green", typeof(Rgb).GetField("Green")!),
            ("blue", typeof(Rgb).GetField("Blue")!)
        }
    );
}