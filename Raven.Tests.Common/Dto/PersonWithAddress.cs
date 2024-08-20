// -----------------------------------------------------------------------
//  <copyright file="PersonWithAddress.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Tests.Common.Dto
{
    public class PersonWithAddress
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; } 
    }
}
