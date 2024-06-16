﻿//HintName: Some.Nested.Namespace.ColorIntSerdeTypeInfo.cs
namespace Some.Nested.Namespace;
internal static class ColorIntSerdeTypeInfo
{
    internal static readonly Serde.TypeInfo TypeInfo = Serde.TypeInfo.Create(
        Serde.TypeInfo.TypeKind.CustomType,
        new (string, System.Reflection.MemberInfo)[] {
("red", typeof(Some.Nested.Namespace.ColorInt).GetField("Red")!),
("green", typeof(Some.Nested.Namespace.ColorInt).GetField("Green")!),
("blue", typeof(Some.Nested.Namespace.ColorInt).GetField("Blue")!)
    });
}