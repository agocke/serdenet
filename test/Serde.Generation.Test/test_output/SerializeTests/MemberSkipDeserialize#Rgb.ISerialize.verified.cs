﻿//HintName: Rgb.ISerialize.cs

#nullable enable
using Serde;

partial struct Rgb : Serde.ISerialize
{
    void Serde.ISerialize.Serialize(ISerializer serializer)
    {
        var type = serializer.SerializeType("Rgb", 3);
        type.SerializeField("red", new ByteWrap(this.Red));
        type.SerializeField("green", new ByteWrap(this.Green));
        type.SerializeField("blue", new ByteWrap(this.Blue));
        type.End();
    }
}