#if DNXCORE50
// -----------------------------------------------------------------------
//  <copyright file="DescriptionAttribute.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace Raven35.Abstractions
{
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
        }
    }
}
#endif