// -----------------------------------------------------------------------
//  <copyright file="TriggerExtensions.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using Raven35.Abstractions.Logging;
using Raven35.Abstractions.MEF;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Database.FileSystem.Plugins
{
    public static class TriggerExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static bool CanReadFile(this OrderedPartCollection<AbstractFileReadTrigger> triggers, string name, RavenJObject metadata, ReadOperation operation)
        {
            foreach (var trigger in triggers)
            {
                var result = trigger.Value.AllowRead(name, metadata, operation);

                switch (result.Veto)
                {
                    case ReadVetoResult.ReadAllow.Allow:
                        break;
                    case ReadVetoResult.ReadAllow.Ignore:
                        if (Log.IsDebugEnabled)
                            Log.Debug("Trigger {0} asked us to ignore {1}", trigger.Value, name);
                        return false;
                    case ReadVetoResult.ReadAllow.Deny:
                        if (Log.IsDebugEnabled)
                            Log.Debug("Trigger {0} denied to read {1} because {2}", trigger.Value, name, result.Reason);
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException(result.Veto.ToString());
                }
            }

            return true;
        }
    }
}
