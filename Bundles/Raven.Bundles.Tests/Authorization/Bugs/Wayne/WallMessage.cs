extern alias client;
using System.Collections.Generic;
using client::Raven35.Bundles.Authorization.Model;

namespace Raven35.Bundles.Tests.Authorization.Bugs.Wayne
{
    public class WallMessage<T>
    {
        public string Id { get; set; }
        public AuthorizationUser Creator { get; set; }
        public T Recipient { get; set; }
        public string MessageBody { get; set; }
        public List<DocumentPermission> Permissions { get; set; }
    }
}
