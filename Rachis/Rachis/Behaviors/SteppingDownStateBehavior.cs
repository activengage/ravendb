// -----------------------------------------------------------------------
//  <copyright file="SteppingDownStateBehavior.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Linq;
using Rachis.Messages;

using Raven35.Abstractions.Logging;

namespace Rachis.Behaviors
{
    public class SteppingDownStateBehavior : LeaderStateBehavior
    {
        private readonly Stopwatch _stepdownDuration;
        private long _lastLogEntryAtTimeOfSteppingDownIndex;

        public SteppingDownStateBehavior(RaftEngine engine)
            : base(engine)
        {
            _stepdownDuration = Stopwatch.StartNew();
            // we are sending this to ourselves because we want to make 
            // sure that we immediately check if we can step down
            Engine.Transport.SendToSelf(new AppendEntriesResponse
            {
                CurrentTerm = Engine.PersistentState.CurrentTerm,
                From = Engine.Name,
                ClusterTopologyId = Engine.CurrentTopology.TopologyId,
                LastLogIndex = Engine.PersistentState.LastLogEntry().Index,
                LeaderId = Engine.Name,
                Message = "Forcing step down evaluation",
                Success = true
            });

            var lastLogEntry = Engine.PersistentState.LastLogEntry();
            _lastLogEntryAtTimeOfSteppingDownIndex = lastLogEntry.Index;
        }

        public override RaftEngineState State
        {
            get { return RaftEngineState.SteppingDown; }
        }

        public override bool CanHaveConfirmedLeader => false;

        public override void Handle(AppendEntriesResponse resp)
        {
            base.Handle(resp);

            var maxIndexOnQuorom = GetMaxIndexOnQuorum();

            if (maxIndexOnQuorom >= _lastLogEntryAtTimeOfSteppingDownIndex)
            {
                _log.Info("Done sending all events to the cluster, can step down gracefully now");
                TransferToBestMatch();
                _lastLogEntryAtTimeOfSteppingDownIndex = long.MaxValue;// only do it once
            }
        }

        private void TransferToBestMatch()
        {
            var bestMatch = _matchIndexes.OrderByDescending(x => x.Value)
                .Select(x => x.Key).FirstOrDefault(x => x != Engine.Name);

            if (bestMatch != null) // this should always be the case, but...
            {
                var nodeConnectionInfo = Engine.CurrentTopology.GetNodeByName(bestMatch);
                if (nodeConnectionInfo != null)
                {
                    Engine.Transport.Send(nodeConnectionInfo, new TimeoutNowRequest
                    {
                        Term = Engine.PersistentState.CurrentTerm,
                        From = Engine.Name,
                        ClusterTopologyId = Engine.CurrentTopology.TopologyId,
                    });
                    _log.Info("Transferring cluster leadership to {0}", bestMatch);
                }
            }

            Engine.SetState(RaftEngineState.FollowerAfterStepDown);
        }

        public override void HandleTimeout()
        {
            base.HandleTimeout();
            if (_stepdownDuration.Elapsed > Engine.Options.MaxStepDownDrainTime)
            {
                _log.Info("Step down has aborted after {0} because this is greater than the max step down time", _stepdownDuration.Elapsed);
                TransferToBestMatch();
            }
        }

        public override void Dispose()
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Disposing of SteppingDownStateBehavior");

            base.Dispose();
            Engine.FinishSteppingDown();
        }
    }
}
