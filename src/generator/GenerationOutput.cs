
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Serde;

/// <summary>
/// The output of a single generation pass.
/// </summary>
public readonly record struct GenerationOutput
{
    public ImmutableArray<Diagnostic> Diagnostics { get;}
    public ImmutableSortedSet<(string FileName, string Content)> Sources { get; }

    public GenerationOutput(
        IEnumerable<Diagnostic> diagnostics,
        IEnumerable<(string fileName, string content)> sources)
    {
        var diagSet = new HashSet<Diagnostic>();
        var diagBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
        foreach (var diag in diagnostics)
        {
            if (diagSet.Add(diag))
            {
                diagBuilder.Add(diag);
            }
        }
        var outputBuilder = ImmutableSortedSet.CreateBuilder<(string, string)>();
        foreach (var source in sources)
        {
            outputBuilder.Add(source);
        }
        Diagnostics = diagBuilder.ToImmutable();
        Sources = outputBuilder.ToImmutable();
    }

    public bool Equals(GenerationOutput other)
    {
        return Diagnostics.AsSpan().SequenceEqual(other.Diagnostics.AsSpan())
            && Sources.SetEquals(other.Sources);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var diag in Diagnostics)
        {
            hash.Add(diag);
        }
        foreach (var source in Sources)
        {
            hash.Add(source);
        }
        return hash.ToHashCode();
    }
}