
using System;
using System.Diagnostics;

namespace Serde;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
public sealed class GenerateSerialize : Attribute
{ }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
public sealed class GenerateDeserialize : Attribute
{ }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
public sealed class GenerateSerde : Attribute
{ }

[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
public sealed class GenerateWrapper : Attribute
{
    public GenerateWrapper(string memberName) { }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SerdeTypeOptions : Attribute
{
    public bool DenyUnknownMembers { get; init; } = false;
    /// <summary>
    /// Override the formatting for members.
    /// </summary>
    public MemberFormat MemberFormat { get; init; } = MemberFormat.None;
    /// <summary>
    /// Pick the constructor used for deserialization. Expects a tuple with the same types as
    /// the desired parameter list of the desired constructor.
    /// </summary>
    public Type? ConstructorSignature { get; init; }
    /// <summary>
    /// The default behavior for null is to skip serialization. Set this to true to force
    /// serialization.
    /// </summary>
    public bool SerializeNull { get; init; } = false;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class SerdeMemberOptions : Attribute
{
    /// <summary>
    /// Throw an exception if the target field is not present when deserializing.  This is the
    /// default behavior for fields of non-nullable types, while the default behavior for nullable
    /// types is to set the field to null.
    /// </summary>
    public bool ThrowIfMissing { get; init; } = false;

    /// <summary>
    /// Use the given name instead of the name of the field or property.
    /// </summary>
    public string? Rename { get; init; } = null;

    /// <summary>
    /// If true, the source generator will pass down the attributes from the member to the
    /// serializer.
    /// </summary>
    public bool ProvideAttributes { get; init; } = false;

    /// <summary>
    /// The default behavior for null is to skip serialization. Set this to true to force
    /// serialization.
    /// </summary>
    public bool SerializeNull { get; init; } = false;
}

public enum MemberFormat : byte
{
    /// <summary>
    /// Use the original name of the member.
    /// </summary>
    None,
    /// <summary>
    /// "PascalCase"
    /// </summary>
    PascalCase,
    /// <summary>
    /// "camelCase"
    /// </summary>
    CamelCase
}