﻿//HintName: Test.RecursiveWrap.IDeserialize.cs

#nullable enable

using System;
using Serde;

namespace Test;

partial class RecursiveWrap : Serde.IDeserializeProvider<Recursive>
{
    static IDeserialize<Recursive> IDeserializeProvider<Recursive>.DeserializeInstance
        => RecursiveWrapDeserializeProxy.Instance;

    sealed partial class RecursiveWrapDeserializeProxy :Serde.IDeserialize<Recursive>
    {
        Recursive Serde.IDeserialize<Recursive>.Deserialize(IDeserializer deserializer)
        {
            Recursive? _l_next = default!;

            byte _r_assignedValid = 0;

            var _l_serdeInfo = global::Serde.SerdeInfoProvider.GetInfo<RecursiveWrap>();
            var typeDeserialize = deserializer.ReadType(_l_serdeInfo);
            int _l_index_;
            while ((_l_index_ = typeDeserialize.TryReadIndex(_l_serdeInfo, out _)) != IDeserializeType.EndOfType)
            {
                switch (_l_index_)
                {
                    case 0:
                        _l_next = typeDeserialize.ReadValue<Recursive?, Test.RecursiveWrap>(_l_index_);
                        _r_assignedValid |= ((byte)1) << 0;
                        break;
                    case Serde.IDeserializeType.IndexNotFound:
                        typeDeserialize.SkipValue();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected index: " + _l_index_);
                }
            }
            if ((_r_assignedValid & 0b0) != 0b0)
            {
                throw Serde.DeserializeException.UnassignedMember();
            }
            var newType = new Recursive() {
                Next = _l_next,
            };

            return newType;
        }
        public static readonly RecursiveWrapDeserializeProxy Instance = new();
        private RecursiveWrapDeserializeProxy() { }

    }
}
