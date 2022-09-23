
using System;
using System.Diagnostics;

namespace Serde;

// Silence warnings about references to Serde types that aren't referenced by the generator
#pragma warning disable CS1574

/// <summary>
/// Generates an implementation of <see cref="Serde.ISerialize" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
#if !SRCGEN
public
#else
internal
#endif
sealed class GenerateSerialize : Attribute
{ }

/// <summary>
/// Generates an implementation of <see cref="Serde.IDeserialize" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
#if !SRCGEN
public
#else
internal
#endif
sealed class GenerateDeserialize : Attribute
{ }

/// <summary>
/// Generates an implementation of both <see cref="Serde.ISerialize" /> and <see cref="Serde.IDeserialize" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
#if !SRCGEN
public
#else
internal
#endif
sealed class GenerateSerde : Attribute
{ }

/// <summary>
/// Generates the equivalent of <see cref="GenerateSerde" />, but delegated to a member of the name
/// passed in as a parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[Conditional("EMIT_GENERATE_SERDE_ATTRIBUTE")]
#if !SRCGEN
public
#else
internal
#endif
sealed class GenerateWrapper : Attribute
{
    /// <summary>
    /// The name of the member used for delegation.
    /// </summary>
    public string MemberName { get; }

    /// <summary>
    /// Constructor for GenerateWrapper.
    /// </summary>
    public GenerateWrapper(string memberName)
    {
        MemberName = memberName;
    }
}
#pragma warning restore CS1574

/// <summary>
/// Set options for the Serde source generator for the current type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
#if !SRCGEN
public
#else
internal
#endif
sealed class SerdeTypeOptions : Attribute
{
    /// <summary>
    /// Throw an exception during deserialization if any members not expected by the current
    /// type are present.
    /// </summary>
    public bool DenyUnknownMembers { get; init; } = false;

    /// <summary>
    /// Override the formatting for members.
    /// </summary>
    public MemberFormat MemberFormat { get; init; } = MemberFormat.CamelCase;

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

/// <summary>
/// Set options for the Serde source generator specific to the current member.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if !SRCGEN
public
#else
internal
#endif
sealed class SerdeMemberOptions : Attribute
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

/// <summary>
/// A enumeration of all possible types of name formatting that the source generator
/// can generate.
/// </summary>
#if !SRCGEN
public
#else
internal
#endif
enum MemberFormat : byte
{
    /// <summary>
    /// "camelCase"
    /// </summary>
    CamelCase,
    /// <summary>
    /// Use the original name of the member.
    /// </summary>
    None,
    /// <summary>
    /// "PascalCase"
    /// </summary>
    PascalCase,
    /// <summary>
    /// "kebab-case"
    /// </summary>
    KebabCase,
}