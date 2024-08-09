using System;

namespace Raven35.Database.Linq
{
    [Obsolete("Use RavenFS instead.")]
    public class AttachmentForIndexing 
    {
        private readonly string key;
        public string Key
        {
            get { return key; }
        }

        public AttachmentForIndexing(string key)
        {
            this.key = key;
        }
    }
}
