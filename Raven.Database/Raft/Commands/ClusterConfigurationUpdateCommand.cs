// -----------------------------------------------------------------------
//  <copyright file="ClusterConfigurationUpdateCommand.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Threading.Tasks;

using Rachis.Commands;

using Raven35.Database.Raft.Dto;

namespace Raven35.Database.Raft.Commands
{
    public class ClusterConfigurationUpdateCommand : Command
    {
        public ClusterConfiguration Configuration { get; set; }

        public static ClusterConfigurationUpdateCommand Create(ClusterConfiguration configuration)
        {
            return new ClusterConfigurationUpdateCommand
                   {
                       Configuration = configuration,
                       Completion = new TaskCompletionSource<object>()
                   };
        }
    }
}
