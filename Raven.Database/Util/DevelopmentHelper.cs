// -----------------------------------------------------------------------
//  <copyright file="DevelopmentHelper.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;

using Raven35.Abstractions;

namespace Raven35.Database.Util
{
    internal static class DevelopmentHelper
    {
        public static void TimeBomb()
        {
            if (SystemTime.UtcNow > new DateTime(2016, 6, 1))
                throw new NotImplementedException("Development time bomb.");
        }
    }
}
