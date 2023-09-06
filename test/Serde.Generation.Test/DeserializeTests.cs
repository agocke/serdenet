
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using VerifyXunit;
using Xunit;
using static Serde.Test.GeneratorTestUtils;

namespace Serde.Test
{
    [UsesVerify]
    public class DeserializeTests
    {
        [Fact]
        public Task NestedExplicitDeserializeWrapper()
        {
            var src = """

using Serde;
using System.Collections.Immutable;
using System.Collections.Specialized;

[GenerateDeserialize(Through = nameof(Value))]
readonly partial record struct SectionWrap(BitVector32.Section Value);

[GenerateDeserialize]
partial struct S
{
    [SerdeMemberOptions(WrapperDeserialize = typeof(ImmutableArrayWrap.DeserializeImpl<BitVector32.Section, SectionWrap>))]
    public ImmutableArray<BitVector32.Section> Sections;
}

""";
            return VerifyMultiFile(src);
        }
        [Fact]
        public Task DeserializeOnlyWrap()
        {
            var src = """
using Serde;
using System.Collections.Specialized;

[GenerateDeserialize(Through = nameof(Value))]
readonly partial record struct SectionWrap(BitVector32.Section Value);

""";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task MemberSkip()
        {
            var src = """
using Serde;
[GenerateDeserialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(Skip = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task MemberSkipSerialize()
        {
            var src = """
using Serde;
[GenerateDeserialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(SkipDeserialize = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task MemberSkipDeserialize()
        {
            var src = """
using Serde;
[GenerateDeserialize]
partial struct Rgb
{
    public byte Red;
    [SerdeMemberOptions(SkipSerialize = true)]
    public byte Green;
    public byte Blue;
}
""";
            return VerifyDeserialize(src);
        }
        [Fact]
        public Task Rgb()
        {
            var src = @"
using Serde;
[GenerateDeserialize]
partial struct Rgb
{
    public byte Red, Green, Blue;
}";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task NullableRefField()
        {
            var src = @"
[Serde.GenerateDeserialize]
partial struct S
{
    public string? F;
}";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task DeserializeMissing()
        {
            var src = @"
using Serde;
[GenerateDeserialize]
readonly partial record struct SetToNull
{
    public string Present { get; init; }
    public string? Missing { get; init; }
    [SerdeMemberOptions(ThrowIfMissing = true)]
    public string? ThrowMissing { get; init; }
} ";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task Array()
        {
            var src = @"
using Serde;
[GenerateDeserialize]
partial class ArrayField
{
    public int[] IntArr = new[] { 1, 2, 3 };
}";
            return VerifyDeserialize(src);
        }

        [Fact]
        public Task EnumMember()
        {
            var src = @"
[Serde.GenerateDeserialize]
partial class C
{
    public ColorInt ColorInt;
    public ColorByte ColorByte;
    public ColorLong ColorLong;
    public ColorULong ColorULong;
}
[Serde.GenerateDeserialize]
public enum ColorInt { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateDeserialize]
public enum ColorByte : byte { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateDeserialize]
public enum ColorLong : long { Red = 3, Green = 5, Blue = 7 }
[Serde.GenerateDeserialize]
public enum ColorULong : ulong { Red = 3, Green = 5, Blue = 7 }
";

            return GeneratorTestUtils.VerifyMultiFile(src);
        }

        private static Task VerifyDeserialize(
            string src,
            [CallerMemberName] string caller = "")
            => VerifyGeneratedCode(src,
                nameof(DeserializeTests),
                caller,
                multiFile: false);
    }
}