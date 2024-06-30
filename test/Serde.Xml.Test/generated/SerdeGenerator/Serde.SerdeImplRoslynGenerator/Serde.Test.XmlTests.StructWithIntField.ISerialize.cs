﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial class XmlTests
    {
        partial record StructWithIntField : Serde.ISerialize<Serde.Test.XmlTests.StructWithIntField>
        {
            void ISerialize<Serde.Test.XmlTests.StructWithIntField>.Serialize(Serde.Test.XmlTests.StructWithIntField value, ISerializer serializer)
            {
                var _l_typeInfo = StructWithIntFieldSerdeTypeInfo.TypeInfo;
                var type = serializer.SerializeType(_l_typeInfo);
                type.SerializeField<int, Int32Wrap>(_l_typeInfo, 0, this.X);
                type.End();
            }
        }
    }
}