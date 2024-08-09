using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven35.Client.Document;

namespace Raven35.NetCore.Tryouts
{
    public class Program
    {

        public class User
        {
            public string Name { get; set; }
        }
        public static void Main(string[] args)
        {
            using (var store = new DocumentStore
            {
                Url = "http://live-test.ravendb.net",
                DefaultDatabase = "Northwind"
            }.Initialize())
            {

                using (var session = store.OpenSession())
                {
                    session.Store(new User {Name="Fiodr"});
                    session.SaveChanges();
                }
            }
        }
    }
}
