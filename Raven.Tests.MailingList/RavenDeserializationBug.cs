using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;
using Raven35.Tests.Helpers;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class RavenDeserializationBug : RavenTestBase
    {
        [Fact]
        public void ShouldBeAbleToIndexChildrenWithUris()
        {
            using (var store = NewDocumentStore())
            {
                store.ExecuteIndex(new IndexChildren());

                var collection =
                    new ContainerType
                        {
                            Children =
                                {
                                    new TypeWithUriProperty {Name = "bar", Address = null},
                                    new TypeWithUriProperty {Name = "foo", Address = new Uri("http://foo")},
                                    new TypeWithUriProperty {Name = "goo", Address = new Uri("http://www.google.com")}
                                }
                        };

                using (var session = store.OpenSession())
                {
                    session.Store(collection);
                    session.SaveChanges();
                }

                WaitForIndexing(store);

                using (var session = store.OpenSession())
                {
                    var results = session
                        .Query<TypeWithUriProperty, IndexChildren>()
                        .Customize(q => q.WaitForNonStaleResults())
                        .OrderBy(x=>x.Name)
                        .ProjectFromIndexFieldsInto<TypeWithUriProperty>()
                        .ToList();
//The previous block will blow up in Raven 2, but not in the previous version. If the TypeWithUriProperty.Address is changed
//to be a string rather than a URI, the test will pass in both versions of RavenDB.
//
//The exception/stack trace in Raven 2.0 is as follows:
//Raven35.Imports.Newtonsoft.Json.JsonSerializationException : Unexpected token while deserializing object: PropertyName. Path 'Name'.
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 266#0
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, Object target) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 653#1
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.PopulateObject(Object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, String id) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 1361#2
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 359#3
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, Object existingValue) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 229#4
//   at Raven35.Imports.Newtonsoft.Json.Serialization.JsonSerializerInternalReader.Deserialize(JsonReader reader, Type objectType, Boolean checkAdditionalContent) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\Serialization\JsonSerializerInternalReader.cs:line 152#5
//   at Raven35.Imports.Newtonsoft.Json.JsonSerializer.DeserializeInternal(JsonReader reader, Type objectType) in c:\Builds\RavenDB-Stable\Imports\Newtonsoft.Json\Src\Newtonsoft.Json\JsonSerializer.cs:line 546#6
//   at Raven35.Client.Document.SessionOperations.QueryOperation.DeserializedResult[T](RavenJObject result) in c:\Builds\RavenDB-Stable\Raven35.Client.Lightweight\Document\SessionOperations\QueryOperation.cs:line 215#7
//   at Raven35.Client.Document.SessionOperations.QueryOperation.Deserialize[T](RavenJObject result) in c:\Builds\RavenDB-Stable\Raven35.Client.Lightweight\Document\SessionOperations\QueryOperation.cs:line 179#8
//   at System.Linq.Enumerable.WhereSelectListIterator`2.MoveNext()
//   at System.Collections.Generic.List`1..ctor(IEnumerable`1 collection)
//   at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)
//   at Raven35.Client.Document.SessionOperations.QueryOperation.Complete[T]() in c:\Builds\RavenDB-Stable\Raven35.Client.Lightweight\Document\SessionOperations\QueryOperation.cs:line 135#9
//   at Raven35.Client.Document.AbstractDocumentQuery`2.GetEnumerator() in c:\Builds\RavenDB-Stable\Raven35.Client.Lightweight\Document\AbstractDocumentQuery.cs:line 751#10
//   at Raven35.Client.Linq.RavenQueryInspector`1.GetEnumerator() in c:\Builds\RavenDB-Stable\Raven35.Client.Lightweight\Linq\RavenQueryInspector.cs:line 100#11
//   at System.Collections.Generic.List`1..ctor(IEnumerable`1 collection)
//   at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)
//   at Tests.RavenDeserializationBug.ShouldBeAbleToIndexChildrenWithUris()

                    Assert.Equal("bar", results[0].Name);
                    Assert.Equal(null, results[0].Address);

                    Assert.Equal("foo", results[1].Name);
                    Assert.Equal(new Uri("http://foo"), results[1].Address);

                    Assert.Equal("goo", results[2].Name);
                    Assert.Equal(new Uri("http://www.google.com"), results[2].Address);
                }
            }
        }

        public class TypeWithUriProperty : IEquatable<TypeWithUriProperty>
        {
            public string Id { get; set; }
            public Uri Address { get; set; }
            public string Name { get; set; }

            #region Equality
            public bool Equals(TypeWithUriProperty other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Id, other.Id)
                       && string.Equals(Name, other.Name)
                       && Equals(Address, other.Address);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TypeWithUriProperty);
            }

            public override int GetHashCode()
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                return hashCode;
            }
            #endregion

            public override string ToString()
            {
                return string.Format("[{0}] {1} = {2}", Id, Name, Address);
            }
        }

        public class ContainerType
        {
            public ContainerType()
            {
                Children = new List<TypeWithUriProperty>();
            }

            public string Id { get; set; }
            public IList<TypeWithUriProperty> Children { get; set; }
        }

        public class IndexChildren : AbstractIndexCreationTask<ContainerType, TypeWithUriProperty>
        {
            public IndexChildren()
            {
                Map = containers => from container in containers
                                    from item in container.Children
                                    select new TypeWithUriProperty
                                               {
                                                   Id = item.Id,
                                                   Name = item.Name,
                                                   Address = item.Address
                                               };

                Store(i => i.Id, FieldStorage.Yes);
                Store(i => i.Name, FieldStorage.Yes);
                Store(i => i.Address, FieldStorage.Yes);
            }
        }
    }
}
