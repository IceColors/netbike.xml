namespace NetBike.Xml.Tests.Samples;

public class BaseClass
{
    public int Test2 { get; set; }
}

public class InheritanceOrder : BaseClass
{
    public int Test1 { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is InheritanceOrder other))
        {
            return false;
        }

        return other.Test1 == Test1 && other.Test2 == Test2;
    }
}

