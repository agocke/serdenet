﻿
#nullable enable
using Serde;

namespace Serde.Test
{
    partial class MemberFormatTests
    {
        partial struct S2 : Serde.ISerialize
        {
            void Serde.ISerialize.Serialize(ISerializer serializer)
            {
                var type = serializer.SerializeType("S2", 2);
                type.SerializeField("X", new Int32Wrap(this.One));
                type.SerializeField("Y", new Int32Wrap(this.TwoWord));
                type.End();
            }
        }
    }
}