using System;
using static NetBike.Xml.Tests.XmlSerializerTests;
using System.Xml.Linq;

namespace NetBike.Xml.Tests.Samples;

public class NullableValueTypeClass
{
    public DateTime? DateTime { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is NullableValueTypeClass other))
        {
            return false;
        }

        return other.DateTime == DateTime;
    }
}