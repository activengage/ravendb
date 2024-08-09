// -----------------------------------------------------------------------
//  <copyright file="Users_Locations.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;
using Raven35.Client.Indexes;
using Raven35.SimulatedWorkload.Model;

namespace Raven35.SimulatedWorkload.Indexes
{
    public class Users_Locations : AbstractIndexCreationTask<User>
    {
        public Users_Locations()
        {
            Map = users =>
                  from user in users
                  select new {user.City, user.State, user.StreetAddress, user.Zip};
        }
    }
}
