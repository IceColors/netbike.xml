using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetBike.Xml.Utilities
{
    internal static class MemberExtensions
    {

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                FieldInfo fieldInfo => fieldInfo.FieldType,
                _ => throw new IndexOutOfRangeException($"memberInfo must be of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}")
            };
        }

        public static IEnumerable<MemberInfo> GetPropertiesAndFields(this Type type)
        {
            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi is PropertyInfo pi ? pi.GetIndexParameters().Length == 0 : mi is FieldInfo);
        }

        public static bool CanRead(this MemberInfo memberInfo){
            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.CanRead,
                FieldInfo fieldInfo => true,
                _ => throw new IndexOutOfRangeException($"memberInfo must be of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}")
            };
        }

        public static bool CanWrite(this MemberInfo memberInfo){
            return memberInfo switch
            {
                PropertyInfo propertyInfo => propertyInfo.CanWrite,
                FieldInfo fieldInfo => !fieldInfo.IsInitOnly,
                _ => throw new IndexOutOfRangeException($"memberInfo must be of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}")
            };
        }
    }
}
