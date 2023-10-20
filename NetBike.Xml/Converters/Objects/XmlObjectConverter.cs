namespace NetBike.Xml.Converters.Objects
{
    using System;
    using System.Xml;
    using NetBike.Xml.Contracts;
    using NetBike.Xml.Utilities;

    public class XmlObjectConverter : IXmlConverter
    {
        public virtual bool CanRead(Type valueType)
        {
            return !valueType.IsBasicType() && valueType.IsActivable();
        }

        public virtual bool CanWrite(Type valueType)
        {
            return !valueType.IsBasicType();
        }

        public object ReadXml(XmlReader reader, XmlSerializationContext context)
        {
            if (context.Member.MappingType != XmlMappingType.Element)
            {
                throw new XmlSerializationException($"XML mapping of \"{context.Member.Name?.LocalName ?? context.ValueType?.Name ?? context.ValueType?.ToString()}\" must be Element.");
            }

            var contract = context.Contract.ToObjectContract();
            var target = this.CreateTarget(contract);

            var propertyInfos = XmlPropertyInfo.GetInfo(contract, reader.NameTable, context);

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    this.ReadProperty(reader, target, propertyInfos, XmlMappingType.Attribute, context);
                }
                while (reader.MoveToNextAttribute());

                reader.MoveToElement();
            }

            if (!reader.IsEmptyElement)
            {
                if (contract.InnerTextProperty != null)
                {
                    var value = context.Deserialize(reader, contract.InnerTextProperty);
                    this.SetValue(target, contract.InnerTextProperty, value);
                }
                else
                {
                    reader.ReadStartElement();

                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            this.ReadProperty(reader, target, propertyInfos, XmlMappingType.Element, context);
                        }
                        else
                        {
                            reader.Read();
                        }
                    }

                    reader.Read();
                }
            }
            else
            {
                reader.Read();
            }

            this.SetDefaultProperties(contract, target, propertyInfos);

            return this.GetResult(target);
        }

        public void WriteXml(XmlWriter writer, object value, XmlSerializationContext context)
        {
            if (value == null)
            {
                return;
            }

            if (context.Member.MappingType != XmlMappingType.Element)
            {
                throw new XmlSerializationException($"XML mapping of \"{context.ValueType}\" must be Element.");
            }

            var contract = context.Contract.ToObjectContract();

            foreach (var property in contract.Properties)
            {
                if (this.CanWriteProperty(property))
                {
                    var propertyValue = this.GetValue(value, property);

                    if (!property.IsCollection)
                    {
                        context.Serialize(writer, propertyValue, property);
                    }
                    else
                    {
                        context.SerializeBody(writer, propertyValue, property);
                    }
                }
            }
        }

        protected virtual bool CanWriteProperty(XmlProperty property)
        {
            return property.HasGetterAndSetter;
        }

        protected virtual object CreateTarget(XmlContract contract)
        {
            return contract.CreateDefault();
        }

        protected virtual object GetResult(object target)
        {
            return target;
        }

        protected virtual object GetValue(object target, XmlProperty property)
        {
            return property.GetValue(target);
        }

        protected virtual void OnUnknownProperty(XmlReader reader, object target, XmlSerializationContext context)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                reader.Skip();
            }
        }

        protected virtual void SetValue(object target, XmlProperty property, object propertyValue)
        {
            property.SetValue(target, propertyValue);
        }

        private void ReadProperty(XmlReader reader, object target, XmlPropertyInfo[] propertyInfos, XmlMappingType mappingType, XmlSerializationContext context)
        {
            for (var i = 0; i < propertyInfos.Length; i++)
            {
                var member = propertyInfos[i].Match(mappingType, reader);

                if (member != null)
                {
                    if (propertyInfos[i].CollectionProxy == null)
                    {
                        var lineNumber = (reader as IXmlLineInfo)?.LineNumber;
                        var value = context.Deserialize(reader, member);
                        context.Settings.OnXmlDeserialized(new XmlDeserializedEventArgs
                        {
                            LineNumber = lineNumber ?? -1,
                            MemberInfo = propertyInfos[i].Property.MemberInfo,
                            XmlName = member.Name.LocalName
                        });
                        this.SetValue(target, propertyInfos[i].Property, value);
                    }
                    else
                    {
                        var lineNumber = (reader as IXmlLineInfo)?.LineNumber;
                        var value = context.Deserialize(reader, propertyInfos[i].Property.Item);
                        context.Settings.OnXmlDeserialized(new XmlDeserializedEventArgs
                        {
                            LineNumber = lineNumber ?? -1,
                            MemberInfo = propertyInfos[i].Property.MemberInfo,
                            XmlName = member.Name.LocalName
                        });
                        propertyInfos[i].CollectionProxy.Add(value);
                    }

                    propertyInfos[i].Assigned = true;
                    return;
                }
            }

            this.OnUnknownProperty(reader, target, context);
        }

        private void SetDefaultProperties(XmlContract contract, object target, XmlPropertyInfo[] propertyInfos)
        {
            for (var i = 0; i < propertyInfos.Length; i++)
            {
                var propertyState = propertyInfos[i];

                if (!propertyState.Assigned)
                {
                    var property = propertyState.Property;

                    if (property.DefaultValue != null && property.MemberInfo.CanWrite())
                    {
                        this.SetValue(target, property, property.DefaultValue);
                    }
                    else if (property.IsRequired)
                    {
                        throw new XmlSerializationException($"Property \"{property.PropertyName}\" of type \"{contract.ValueType}\" is required.");
                    }
                }
                else if (propertyState.CollectionProxy != null)
                {
                    var collection = propertyState.CollectionProxy.GetResult();
                    this.SetValue(target, propertyState.Property, collection);
                }
            }
        }
    }
}