﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial class GenericWrapperTests
    {
        partial record struct CustomArrayWrapExplicitOnType : Serde.ISerialize<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>
        {
            void ISerialize<Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType>.Serialize(Serde.Test.GenericWrapperTests.CustomArrayWrapExplicitOnType value, ISerializer serializer)
            {
                var _l_typeInfo = CustomArrayWrapExplicitOnTypeSerdeTypeInfo.TypeInfo;
                var type = serializer.SerializeType(_l_typeInfo);
                type.SerializeField<Serde.Test.GenericWrapperTests.CustomImArray2<int>, Serde.Test.GenericWrapperTests.CustomImArray2Wrap.SerializeImpl<int, Int32Wrap>>(_l_typeInfo, 0, value.A);
                type.End();
            }
        }
    }
}