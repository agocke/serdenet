
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using VerifyXunit;
using Xunit;
using static Serde.Test.GeneratorTestUtils;

namespace Serde.Test
{
    public class SerializeTests
    {
        [Fact]
        public Task NestedExplicitSerializeWrapper()
        {
            var src = """

using Serde;
using System.Collections.Immutable;
using System.Runtime.InteropServices.ComTypes;

[GenerateSerde(Through = nameof(Value))]
readonly partial record struct OPTSWrap(BIND_OPTS Value);

[GenerateSerde]
partial struct S
{
    [SerdeMemberOptions(
        WrapperSerialize = typeof(ImmutableArrayWrap.SerializeImpl<BIND_OPTS, OPTSWrap>),
        WrapperDeserialize = typeof(ImmutableArrayWrap.DeserializeImpl<BIND_OPTS, OPTSWrap>))]
    public ImmutableArray<BIND_OPTS> Opts;
}

""";
            return VerifyMultiFile(src);
        }

        [Fact]
        public Task SerializeOnlyWrapper()
        {
            var src = """
using Serde;
using System.Collections.Specialized;

[GenerateSerialize(Through = nameof(Value))]
readonly partial record struct SectionWrap(BitVector32.Section Value);

""";
            return VerifyMultiFile(src);
        }

        [Fact]
        public Task MemberSkip()
        {
            var src = """
using Serde;
[GenerateSerialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(Skip = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifySerialize(src);
        }

        [Fact]
        public Task MemberSkipSerialize()
        {
            var src = """
using Serde;
[GenerateSerialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(SkipSerialize = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifySerialize(src);
        }

        [Fact]
        public Task MemberSkipDeserialize()
        {
            var src = """
using Serde;
[GenerateSerialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(SkipDeserialize = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifySerialize(src);
        }

        [Fact]
        public Task Rgb()
        {
            var src = @"
using Serde;
[GenerateSerialize]
partial struct Rgb
{
    public byte Red, Green, Blue;
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task NullableRefFields()
        {
            var src = @"
using System;
using Serde;

[GenerateSerialize]
partial struct S<T1, T2, T3, T4, T5>
    where T1 : ISerialize<T1>
    where T2 : ISerialize<T2>?
    where T3 : class, ISerialize<T3>
    where T4 : class?, ISerialize<T4>
{
    public string? FS;
    public T1 F1;
    public T2 F2;
    public T3? F3;
    public T4 F4;
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task NullableFields()
        {
            var src = """
using Serde;
[GenerateSerialize]
partial struct S<T1, T2, TSerialize>
    where T1 : int?
    where T2 : TSerialize?
    where TSerialize : struct, ISerialize<TSerialize>
{
    public int? FI;
    public T1 F1;
    public T2 F2;
    public TSerialize? F3;
}
""";
            return VerifySerialize(src);
        }

        [Fact]
        public Task TypeDoesntImplementISerialize()
        {
            var src = @"
using Serde;
[GenerateSerialize]
partial struct S1
{
    public S2 X;
}
struct S2 { }";
            return VerifySerialize(src);
        }

        [Fact]
        public Task TypeNotPartial()
        {
            var src = @"
using Serde;
[GenerateSerialize]
struct S { }
[GenerateSerialize]
class C { }";
            return VerifyDiagnostics(src);
        }

        [Fact]
        public Task TypeWithArray()
        {
            var src = @"
using Serde;
[GenerateSerialize]
partial class C
{
    public readonly int[] IntArr = new[] { 1, 2, 3 };
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task NestedArray()
        {
            var src = @"
using Serde;
[GenerateSerialize]
partial class C
{
    public readonly int[][] NestedArr = new[] { new[] { 1 }, new[] { 2 } };
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task NestedArray2()
        {
            var src = @"
using Serde;
[GenerateSerialize]
partial class C
{
    public readonly int[][] NestedArr = new int[][] { };
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task ArrayOfGenerateSerialize()
        {
            var src = @"
using Serde;

partial class TestCase15
{
    [GenerateSerialize]
    public partial class Class0
    {
        public Class1[] Field0 = new Class1[]{new Class1()};
        public bool[] Field1 = new bool[]{false};
    }

    [GenerateSerialize]
    public partial class Class1
    {
        public int Field0 = int.MaxValue;
        public byte Field1 = byte.MaxValue;
    }
}";

            return VerifyMultiFile(src);
        }

        [Fact]
        public Task DictionaryGenerate()
        {
            var src = @"
using Serde;
using System.Collections.Generic;

[GenerateSerialize]
partial class C
{
    public readonly Dictionary<string, int> Map = new Dictionary<string, int>()
    {
        [""abc""] = 5,
        [""def""] = 3
    };
}
";
            return VerifySerialize(src);
        }

        [Fact]
        public Task DictionaryGenerate2()
        {
            var src = @"
using Serde;
using System.Collections.Generic;

[GenerateSerialize]
partial record C(int X);

[GenerateSerialize]
partial class C2
{
    public readonly Dictionary<string, C> Map = new Dictionary<string, C>()
    {
        [""abc""] = new C(11),
        [""def""] = new C(3)
    };
}
";
            return VerifyMultiFile(src);
        }

        [Fact]
        public Task IDictionaryImplGenerate()
        {
            var src = @"
using System;
using System.Collections;
using System.Collections.Generic;
using Serde;

record R(Dictionary<string, int> D) : IDictionary<string, int>
{
    public int this[string key] { get => ((IDictionary<string, int>)D)[key]; set => ((IDictionary<string, int>)D)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, int>)D).Keys;

    public ICollection<int> Values => ((IDictionary<string, int>)D).Values;

    public int Count => ((ICollection<KeyValuePair<string, int>>)D).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, int>>)D).IsReadOnly;

    public void Add(string key, int value)
    {
        ((IDictionary<string, int>)D).Add(key, value);
    }

    public void Add(KeyValuePair<string, int> item)
    {
        ((ICollection<KeyValuePair<string, int>>)D).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<string, int>>)D).Clear();
    }

    public bool Contains(KeyValuePair<string, int> item)
    {
        return ((ICollection<KeyValuePair<string, int>>)D).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, int>)D).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, int>>)D).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, int>>)D).GetEnumerator();
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, int>)D).Remove(key);
    }

    public bool Remove(KeyValuePair<string, int> item)
    {
        return ((ICollection<KeyValuePair<string, int>>)D).Remove(item);
    }

    public bool TryGetValue(string key, out int value)
    {
        return ((IDictionary<string, int>)D).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)D).GetEnumerator();
    }
}
[GenerateSerialize]
partial class C
{
    public R RDictionary;
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task ExplicitWrapper()
        {
            var src = @"
using Serde;

public struct S
{
    public int X;
    public int Y;
    public S(int x, int y)
    {
        X = x;
        Y = y;
    }
}
public struct SWrap : ISerialize<S>
{
    void ISerialize<S>.Serialize(S value, ISerializer serializer)
    {
        serializer.SerializeI32(value.X);
        serializer.SerializeI32(value.Y);
    }
}
[GenerateSerialize]
partial class C
{
    [SerdeWrap(typeof(SWrap))]
    public S S = new S();
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task ExplicitGenericWrapper()
        {
            var src = @"
using Serde;

public struct S<T>
{
    public T Field;
    public S(T f)
    {
        Field = f;
    }
}
public static class SWrap
{
    internal static class SWrapTypeInfo
    {
        public static readonly Serde.TypeInfo TypeInfo = Serde.TypeInfo.Create(
            ""S"",
            Serde.TypeInfo.TypeKind.CustomType,
            new (string, System.Reflection.MemberInfo)[] { (""s"", typeof(S<>).GetField(""Field"")!) });
    }
    public readonly struct SerializeImpl<T, TWrap> : ISerialize<S<T>>
        where TWrap : struct, ISerialize<T>
    {
        void ISerialize<S<T>>.Serialize(S<T> value, ISerializer serializer)
        {
            var _l_typeInfo = SWrapTypeInfo.TypeInfo;
            var type = serializer.SerializeType(_l_typeInfo);
            type.SerializeField<T, TWrap>(_l_typeInfo, 0, value.Field);
            type.End();
        }
    }
}
[GenerateSerialize]
partial class C
{
    [SerdeWrap(typeof(SWrap))]
    public S<int> S = new S<int>(5);
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task WrongGenericWrapperForm()
        {
            var src = @"
using Serde;

public struct S<T>
{
    public T Field;
    public S(T f)
    {
        Field = f;
    }
}
public readonly struct SWrap<T, TWrap>
    : ISerialize<T>
    where TWrap : struct, ISerialize<T>
{
    private static readonly Serde.TypeInfo s_typeInfo = Serde.TypeInfo.Create(
        ""S"",
        Serde.TypeInfo.TypeKind.CustomType,
        new (string, System.Reflection.MemberInfo)[] { (""s"", typeof(S<>).GetField(""Field"")!) });

    void ISerialize<T>.Serialize(T value, ISerializer serializer)
    {
        var _l_typeInfo = s_typeInfo;
        var type = serializer.SerializeType(_l_typeInfo);
        type.SerializeField<T, TWrap>(_l_typeInfo, 0, value);
        type.End();
    }
}
[GenerateSerialize]
partial class C
{
    [SerdeWrap(typeof(SWrap<,>))]
    public S<int> S = new S<int>(5);
}";
            return VerifySerialize(src);
        }

        [Fact]
        public Task EnumMember()
        {
            var src = @"
namespace Some.Nested.Namespace;

[Serde.GenerateSerialize]
partial class C
{
    public ColorInt ColorInt;
    public ColorByte ColorByte;
    public ColorLong ColorLong;
    public ColorULong ColorULong;
}
[Serde.GenerateSerialize]
public enum ColorInt { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateSerialize]
public enum ColorByte : byte { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateSerialize]
public enum ColorLong : long { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateSerialize]
public enum ColorULong : ulong { Red = 3, Green = 5, Blue = 7 }
";

            return VerifyMultiFile(src);
        }

        [Fact]
        public Task NestedEnumWrapper()
        {
            var src = """
using Serde;

[GenerateSerialize]
partial class C
{
    public Rgb? ColorOpt;
}

[GenerateSerialize]
public enum Rgb { Red, Green, Blue }
""";
            return VerifyMultiFile(src);
        }

        [Fact]
        public Task NestedPartialClasses()
        {
            var src = """
using Serde;

partial class A
{
    private partial class B
    {
        private partial class C
        {
            [GenerateSerialize]
            private partial class D
            {
                public int Field;
            }
        }
    }
}
""";
            return VerifySerialize(src);
        }

        private static Task VerifySerialize(
            string src,
            [CallerMemberName] string? callerName = null)
        {
            Assert.NotNull(callerName);
            return VerifyGeneratedCode(src, nameof(SerializeTests), callerName, multiFile: false);
        }
    }
}