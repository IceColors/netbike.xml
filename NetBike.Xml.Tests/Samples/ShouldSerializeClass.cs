using System.Xml.Linq;

namespace NetBike.Xml.Tests.Samples;

public class ShouldSerializeClass
{
    public int Test1 { get; set; }
    public int Test2 { get; set; }

    public bool ShouldSerializeTest2()
    {
        return false;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ShouldSerializeClass other))
        {
            return false;
        }

        return other.Test1 == Test1 && other.Test2 == Test2;
    }
}