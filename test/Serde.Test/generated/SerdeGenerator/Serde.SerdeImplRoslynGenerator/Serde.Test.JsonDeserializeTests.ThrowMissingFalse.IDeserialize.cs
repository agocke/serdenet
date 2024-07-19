﻿
#nullable enable
using System;
using Serde;

namespace Serde.Test
{
    partial class JsonDeserializeTests
    {
        partial record ThrowMissingFalse : Serde.IDeserialize<Serde.Test.JsonDeserializeTests.ThrowMissingFalse>
        {
            static Serde.Test.JsonDeserializeTests.ThrowMissingFalse Serde.IDeserialize<Serde.Test.JsonDeserializeTests.ThrowMissingFalse>.Deserialize(IDeserializer deserializer)
            {
                string _l_present = default !;
                bool _l_missing = default !;
                byte _r_assignedValid = 0;
                var _l_serdeInfo = global::Serde.SerdeInfoProvider.GetInfo<ThrowMissingFalse>();
                var typeDeserialize = deserializer.DeserializeType(_l_serdeInfo);
                int _l_index_;
                while ((_l_index_ = typeDeserialize.TryReadIndex(_l_serdeInfo, out var _l_errorName)) != IDeserializeType.EndOfType)
                {
                    switch (_l_index_)
                    {
                        case 0:
                            _l_present = typeDeserialize.ReadValue<string, global::Serde.StringWrap>(_l_index_);
                            _r_assignedValid |= ((byte)1) << 0;
                            break;
                        case 1:
                            _l_missing = typeDeserialize.ReadValue<bool, global::Serde.BoolWrap>(_l_index_);
                            _r_assignedValid |= ((byte)1) << 1;
                            break;
                        case Serde.IDeserializeType.IndexNotFound:
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected index: " + _l_index_);
                    }
                }

                if ((_r_assignedValid & 0b1) != 0b1)
                {
                    throw new Serde.InvalidDeserializeValueException("Not all members were assigned");
                }

                var newType = new Serde.Test.JsonDeserializeTests.ThrowMissingFalse()
                {
                    Present = _l_present,
                    Missing = _l_missing,
                };
                return newType;
            }
        }
    }
}