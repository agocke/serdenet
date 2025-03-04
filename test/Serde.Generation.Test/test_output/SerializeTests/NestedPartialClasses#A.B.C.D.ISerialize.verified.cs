﻿//HintName: A.B.C.D.ISerialize.cs

#nullable enable

using System;
using Serde;
partial class A
{
    partial class B
    {
        partial class C
        {
            partial class D : Serde.ISerializeProvider<A.B.C.D>
            {
                static ISerialize<A.B.C.D> ISerializeProvider<A.B.C.D>.SerializeInstance
                    => DSerializeProxy.Instance;

                sealed partial class DSerializeProxy :Serde.ISerialize<A.B.C.D>
                {
                    void global::Serde.ISerialize<A.B.C.D>.Serialize(A.B.C.D value, global::Serde.ISerializer serializer)
                    {
                        var _l_info = global::Serde.SerdeInfoProvider.GetInfo<D>();
                        var _l_type = serializer.WriteType(_l_info);
                        _l_type.WriteI32(_l_info, 0, value.Field);
                        _l_type.End(_l_info);
                    }
                    public static readonly DSerializeProxy Instance = new();
                    private DSerializeProxy() { }

                }
            }
        }
    }
}
