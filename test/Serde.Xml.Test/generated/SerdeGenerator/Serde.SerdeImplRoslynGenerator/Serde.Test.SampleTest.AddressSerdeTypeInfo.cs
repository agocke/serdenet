﻿namespace Serde.Test;
partial class SampleTest
{
    internal static class AddressSerdeTypeInfo
{
    internal static readonly Serde.TypeInfo TypeInfo = Serde.TypeInfo.Create(new (string, System.Reflection.MemberInfo)[] {
("Name", typeof(Address).GetField("Name")!),
("Line1", typeof(Address).GetField("Line1")!),
("City", typeof(Address).GetField("City")!),
("State", typeof(Address).GetField("State")!),
("Zip", typeof(Address).GetField("Zip")!)
    });
}
}