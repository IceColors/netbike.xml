namespace NetBike.Xml.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using NetBike.Xml.Contracts;
    using NetBike.Xml.Tests.Samples;
    using NetBike.XmlUnit.NUnitAdapter;
    using NUnit.Framework;
    using XmlSerializer = NetBike.Xml.XmlSerializer;

    [TestFixture]
    public class XmlSerializerTests
    {
        [Test]
        public void SerializeFooTest()
        {
            var actual = GetSerializer().ToXml(new Foo
            {
                Id = 1,
                Name = "test"
            });

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <name>test</name>
</foo>";
            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void DeserializeFooTest()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <name>test</name>
</foo>";
            var expected = new Foo
            {
                Id = 1,
                Name = "test"
            };

            var actual = GetSerializer().ParseXml<Foo>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeFooWithoutNamespacesTest()
        {
            var serializer = GetSerializer();
            serializer.Settings.Namespaces.Clear();

            var actual = serializer.ToXml(new Foo
            {
                Id = 1,
                Name = "test"
            });

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo>
  <id>1</id>
  <name>test</name>
</foo>";
            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void SerializeNullTest()
        {
            var serializer = GetSerializer();
            serializer.Settings.NullValueHandling = XmlNullValueHandling.Include;

            var actual = serializer.ToXml<Foo>(null);

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xsi:nil=""true"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" />";

            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void DeserializeNullTest()
        {
            var serializer = new XmlSerializer
            {
                Settings =
                {
                    NullValueHandling = XmlNullValueHandling.Include
                }
            };

            var xml = @"<foo xsi:nil=""true"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" />";
            var actual = serializer.ParseXml<Foo>(xml);

            Assert.IsNull(actual);
        }

        [Test]
        public void SerializeEnumTest()
        {
            var value = new Foo
            {
                Id = 1,
                Name = "test",
                EnumValue = FooEnum.Value1
            };

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <name>test</name>
  <enumValue>value1</enumValue>
</foo>";

            var actual = GetSerializer().ToXml(value);

            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void DeserializeEnumTest()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <name>test</name>
  <enumValue>value1</enumValue>
</foo>";
            var expected = new Foo
            {
                Id = 1,
                Name = "test",
                EnumValue = FooEnum.Value1
            };

            var actual = GetSerializer().ParseXml<Foo>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeFooAsInterfaceTest()
        {
            var serializer = GetSerializer();
            serializer.Settings.OmitXmlDeclaration = true;

            var value = new Foo
            {
                Id = 1,
                Name = "test"
            };

            var expected = @"
<ifoo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
     xsi:type=""NetBike.Xml.Tests.Samples.Foo"">
  <id>1</id>
  <name>test</name>
</ifoo>";
            var actual = serializer.ToXml<IFoo>(value);

            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void DeserializeFooAsInterfaceTest()
        {
            var xml = @"
<foo xsi:type=""NetBike.Xml.Tests.Samples.Foo"" 
     xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <name>foo</name>
</foo>";

            var expected = new Foo
            {
                Id = 1,
                Name = "foo"
            };

            var actual = GetSerializer().ParseXml<IFoo>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeserializeWithUnknownPropertiesTest()
        {
            var xml = @"<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <id>1</id>
  <unknown id=""2""><name>test</name></unknown>
  <name>test</name>
  <test />
</foo>";
            var expected = new Foo
            {
                Id = 1,
                Name = "test"
            };

            var actual = GetSerializer().ParseXml<Foo>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeFooInNamespaceTests()
        {
            var serializer = new XmlSerializer();

            var actual = serializer.ToXml(new SampleClass
            {
                Id = 1,
                Name = "test"
            });

            var expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" p2:id=""1"" xmlns:p2=""http://example.org/id"" xmlns=""http://example.org"">
    <name>test</name>
</foo>";
            Assert.That(actual, IsXml.Equals(expected));
        }

        [Test]
        public void DeserializeFooInNamespaceTest()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<foo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" p2:id=""1"" xmlns:p2=""http://example.org/id"" xmlns=""http://example.org"">
    <name>test</name>
</foo>";
            var expected = new SampleClass
            {
                Id = 1,
                Name = "test"
            };

            var actual = GetSerializer().ParseXml<SampleClass>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeEnumerableTest()
        {
            var value = new List<Foo>
            {
                new Foo { Id = 1 },
                new Foo { Id = 2 }
            };

            var actual = GetSerializer().ToXml<IEnumerable<Foo>>(value);
            var expected = $@"
<ienumerable xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:type=""{value.GetType().FullName}"">
  <foo>
    <id>1</id>
  </foo>
  <foo>
    <id>2</id>
</foo>
</ienumerable>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeEnumerableWithoutTypeHandlingTest()
        {
            var value = new List<Foo>
            {
                new Foo { Id = 1 },
                new Foo { Id = 2 }
            };

            var serializer = GetSerializer();
            serializer.Settings.TypeHandling = XmlTypeHandling.None;

            var actual = serializer.ToXml<IEnumerable<Foo>>(value);
            var expected = "<ienumerable><foo><id>1</id></foo><foo><id>2</id></foo></ienumerable>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeFooCollectionWithFooBarTypeTest()
        {
            var value = new List<Foo>
            {
                new Foo { Id = 1 },
                new FooBar { Id = 2, Description = "test" }
            };

            var actual = GetSerializer().ToXml(value);
            var expected = @"<list xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><foo><id>1</id></foo><foo xsi:type=""NetBike.Xml.Tests.Samples.FooBar""><id>2</id><description>test</description></foo></list>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeFooCollectionWithFooBarTypeAndWithoutTypeHandlingTest()
        {
            var value = new List<Foo>
            {
                new Foo { Id = 1 },
                new FooBar { Id = 2, Description = "test" }
            };

            var serializer = GetSerializer();
            serializer.Settings.TypeHandling = XmlTypeHandling.None;
            var actual = serializer.ToXml(value);
            var expected = @"<list xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><foo><id>1</id></foo><foo><id>2</id></foo></list>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeFooContainerTest()
        {
            var value = new FooContainer
            {
                Foo = new FooBar
                {
                    Id = 1,
                    Description = "test"
                }
            };

            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected = @"<fooContainer xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><foo xsi:type=""NetBike.Xml.Tests.Samples.FooBar""><id>1</id><description>test</description></foo></fooContainer>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeFooContainerWithoutTypeHandlingTest()
        {
            var value = new FooContainer
            {
                Foo = new FooBar
                {
                    Id = 1,
                    Description = "test"
                }
            };

            var serializer = GetSerializer();
            serializer.Settings.TypeHandling = XmlTypeHandling.None;
            var actual = serializer.ToXml(value);
            var expected = @"<fooContainer xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><foo><id>1</id></foo></fooContainer>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void DeserializeDateTimeTest()
        {
            var xml = @"<fooDateTime xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><DateTimeValue>2011-11-10T01:02:03</DateTimeValue><DateValue>2011-11-10</DateValue></fooDateTime>";
            var expected = new FooDateTime
            {
                DateValue = new DateTime(2011, 11, 10),
                DateTimeValue = new DateTime(2011, 11, 10, 1, 2, 3)
            };

            var actual = GetSerializer().ParseXml<FooDateTime>(xml);

            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void SerializeDateTimeTest()
        {
            var value = new FooDateTime
            {
                DateValue = new DateTime(2011, 11, 10),
                DateTimeValue = new DateTime(2011, 11, 10, 1, 2, 3)
            };
            
            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected =
                @"<fooDateTime xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><DateTimeValue>2011-11-10T01:02:03</DateTimeValue><DateValue>2011-11-10</DateValue></fooDateTime>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void SerializeNullableValueType()
        {
            var value = new NullableValueTypeClass()
            {
                DateTime = new DateTime(2024, 01, 23)
            };

            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected = @"<nullableValueTypeClass xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><dateTime>2024-01-23T00:00:00</dateTime></nullableValueTypeClass>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void DeserializeNullableValueType()
        {
            var xml = @"<nullableValueTypeClass xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><dateTime>2024-01-23T00:00:00</dateTime></nullableValueTypeClass>";
            var expected = new NullableValueTypeClass()
            {
                DateTime = new DateTime(2024, 01, 23)
            };

            var actual = GetSerializer().ParseXml<NullableValueTypeClass>(xml);


            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeStructTest()
        {
            var value = new Baz
            {
                Id = 5,
            };

            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected = @"<baz xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><id>5</id></baz>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void DeserializeStructTest()
        {
            var xml = @"<baz xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><id>5</id></baz>";
            var expected = new Baz
            {
                Id = 5,
            };

            var actual = GetSerializer().ParseXml<Baz>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeShouldSerializeClass()
        {
            var value = new ShouldSerializeClass()
            {
                Test1 = 5,
                Test2 = 6
            };

            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected = @"<shouldSerializeClass xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><test1>5</test1></shouldSerializeClass>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void DeserializeShouldSerializeClass()
        {
            var xml = @"<shouldSerializeClass xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><test1>5</test1><test2>6</test2></shouldSerializeClass>";
            var expected = new ShouldSerializeClass
            {
                Test1 = 5,
                Test2 = 6
            };

            var actual = GetSerializer().ParseXml<ShouldSerializeClass>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SerializeInheritanceOrder()
        {
            var value = new InheritanceOrder()
            {
                Test1 = 5,
                Test2 = 6
            };

            var serializer = GetSerializer();
            var actual = serializer.ToXml(value);
            var expected = @"<inheritanceOrder xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><test2>6</test2><test1>5</test1></inheritanceOrder>";

            Assert.That(actual, IsXml.Equals(expected).WithIgnoreDeclaration());
        }

        [Test]
        public void DeserializeInheritanceOrder()
        {
            var xml = @"<inheritanceOrder xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><test2>6</test2><test1>5</test1></inheritanceOrder>";
            var expected = new InheritanceOrder
            {
                Test1 = 5,
                Test2 = 6
            };

            var actual = GetSerializer().ParseXml<InheritanceOrder>(xml);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeserializeTestFieldsTest()
        {
            string xml = @"<TestFields xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><a>2</a><MyProperty>5</MyProperty></TestFields>";
            var expected = new TestFields
            {
                a = 2,
                MyProperty = 5
            };

            var serializer = new XmlSerializer();
            
            var actual = serializer.ParseXml<TestFields>(xml);

            Assert.AreEqual(expected, actual);
        }

        public class TestFields{
            public int MyProperty { get; set; }
            public int a;

            public override bool Equals(object obj)
            {
                if (!(obj is TestFields other))
                {
                    return false;
                }

                return other.MyProperty == MyProperty && other.a == a;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private XmlSerializer GetSerializer()
        {
            var settings = new XmlSerializerSettings
            {
                ContractResolver = new XmlContractResolver(NamingConventions.CamelCase)
            };

            return new XmlSerializer(settings);
        }

        [XmlRoot(ElementName = "foo", Namespace = "http://example.org")]
        public class SampleClass
        {
            [XmlAttribute(AttributeName = "id", Namespace = "http://example.org/id")]
            public int Id { get; set; }

            [XmlElement("name")]
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is SampleClass other))
                {
                    return false;
                }

                return other.Id == Id && other.Name == Name;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}