namespace NetBike.Xml.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using NetBike.Xml.Utilities;

    public class XmlProperty : XmlMember
    {
        private Action<object, object> setter;
        private Func<object, object> getter;

        public XmlProperty(
            MemberInfo memberInfo,
            XmlName name,
            XmlMappingType mappingType = XmlMappingType.Element,
            bool isRequired = false,
            XmlTypeHandling? typeHandling = null,
            XmlNullValueHandling? nullValueHandling = null,
            XmlDefaultValueHandling? defaultValueHandling = null,
            object defaultValue = null,
            XmlItem item = null,
            IEnumerable<XmlKnownType> knownTypes = null,
            bool isCollection = false,
            int order = -1,
            string dataType = null)
            : base(memberInfo.GetMemberType(), name, mappingType, typeHandling, nullValueHandling, defaultValueHandling, defaultValue, item, knownTypes, dataType)
        {
            if (isCollection)
            {
                if (!memberInfo.GetMemberType().IsEnumerable())
                {
                    throw new ArgumentException("Collection flag is available only for the IEnumerable type.");
                }

                this.IsCollection = true;
            }

            this.MemberInfo = memberInfo;
            this.IsRequired = isRequired;
            this.Order = order;
            this.HasGetterAndSetter = memberInfo.CanRead() && memberInfo.CanWrite();
        }

        public MemberInfo MemberInfo { get; }

        public string PropertyName => this.MemberInfo.Name;

        public bool IsRequired { get; }

        public bool IsCollection { get; }

        public int Order { get; }

        internal bool HasGetterAndSetter { get; }

        internal object GetValue(object target)
        {
            getter ??= MemberInfo switch
            {
                PropertyInfo propertyInfo => DynamicWrapperFactory.CreateGetter(propertyInfo),
                FieldInfo fieldInfo => x => fieldInfo.GetValue(x),
                _ => this.getter
            };

            return getter?.Invoke(target);
        }

        internal void SetValue(object target, object value)
        {
            setter ??= MemberInfo switch
            {
                PropertyInfo propertyInfo => DynamicWrapperFactory.CreateSetter(propertyInfo),
                FieldInfo fieldInfo => (obj, val) => fieldInfo.SetValue(obj, val),
                _ => setter
            };
            if(value != null)
                setter?.Invoke(target, value);
        }
    }
}