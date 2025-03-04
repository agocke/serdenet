﻿//HintName: Rgb.IDeserialize.cs

#nullable enable

using System;
using Serde;
partial struct Rgb : Serde.IDeserializeProvider<Rgb>
{
    static IDeserialize<Rgb> IDeserializeProvider<Rgb>.DeserializeInstance
        => RgbDeserializeProxy.Instance;

    sealed partial class RgbDeserializeProxy :Serde.IDeserialize<Rgb>
    {
        Rgb Serde.IDeserialize<Rgb>.Deserialize(IDeserializer deserializer)
        {
            byte _l_red = default!;
            byte _l_blue = default!;

            byte _r_assignedValid = 0;

            var _l_serdeInfo = global::Serde.SerdeInfoProvider.GetInfo<Rgb>();
            var typeDeserialize = deserializer.ReadType(_l_serdeInfo);
            int _l_index_;
            while ((_l_index_ = typeDeserialize.TryReadIndex(_l_serdeInfo, out _)) != IDeserializeType.EndOfType)
            {
                switch (_l_index_)
                {
                    case 0:
                        _l_red = typeDeserialize.ReadByte(_l_index_);
                        _r_assignedValid |= ((byte)1) << 0;
                        break;
                    case 1:
                        _l_blue = typeDeserialize.ReadByte(_l_index_);
                        _r_assignedValid |= ((byte)1) << 1;
                        break;
                    case Serde.IDeserializeType.IndexNotFound:
                        typeDeserialize.SkipValue();
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected index: " + _l_index_);
                }
            }
            if ((_r_assignedValid & 0b11) != 0b11)
            {
                throw Serde.DeserializeException.UnassignedMember();
            }
            var newType = new Rgb() {
                Red = _l_red,
                Blue = _l_blue,
            };

            return newType;
        }
        public static readonly RgbDeserializeProxy Instance = new();
        private RgbDeserializeProxy() { }

    }
}
