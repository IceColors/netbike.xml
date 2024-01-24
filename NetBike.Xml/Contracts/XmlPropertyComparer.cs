using System.Collections.Generic;

namespace NetBike.Xml.Contracts;

internal sealed class XmlPropertyComparer : IComparer<XmlProperty>
{
    internal static readonly IComparer<XmlProperty> Instance = new XmlPropertyComparer();

    public int Compare(XmlProperty x, XmlProperty y)
    {
        if (x.MappingType == XmlMappingType.Attribute && y.MappingType != XmlMappingType.Attribute)
            return -1;

        if (x.MappingType != XmlMappingType.Attribute && y.MappingType == XmlMappingType.Attribute)
            return 1;

        if (x.Order != -1 || y.Order != -1)
            return x.Order.CompareTo(y.Order);

        if (x.MemberInfo.DeclaringType.IsSubclassOf(y.MemberInfo.DeclaringType))
            return 1;

        if (y.MemberInfo.DeclaringType.IsSubclassOf(x.MemberInfo.DeclaringType))
            return -1;

        return x.MemberInfo.MetadataToken.CompareTo(y.MemberInfo.MetadataToken);
    }
}