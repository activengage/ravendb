//-----------------------------------------------------------------------
// <copyright file="OperationPermission.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Bundles.Authorization.Model
{
    public class OperationPermission : IPermission
    {
        public string Operation { get; set; }
        public List<string> Tags { get; set; }
        public bool Allow { get; set; }
        public int Priority { get; set; }

        public OperationPermission()
        {
            Tags = new List<string>();
        }

        [JsonIgnore]
        public string Explain
        {
            get
            {
                return string.Format("Operation: {0}, Tags: {1}, Allow: {2}, Priority: {3}", Operation, string.Join(", ", Tags ?? new List<string>()), Allow, Priority);
            }
        }

        public bool DeepEquals(OperationPermission other)
        {
            if(other == null)
                return false;

            if (Operation != other.Operation)
                return false;

            if(Allow != other.Allow)
                return false;

            if(Priority != other.Priority)
                return false;

            if(Tags.Count != other.Tags.Count)
                return false;

            for (int i = 0; i < Tags.Count; i++)
            {
                if(Tags[i] != other.Tags[i])
                    return false;
            }

            return true;
        }
    }
}
