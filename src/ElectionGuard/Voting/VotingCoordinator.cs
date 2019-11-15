using System;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.Utility;
using ElectionGuard.SDK.Voting.Coordinator;
using ElectionGuard.SDK.Voting.Messages;

namespace ElectionGuard.SDK.Voting
{
    public class VotingCoordinator : SafePointer, IDisposable
    {
        private readonly UIntPtr _coordinator;

        public VotingCoordinator(int numberOfSelections)
        {
            var response = CoordinatorApi.NewCoordinator(Convert.ToUInt32(numberOfSelections));
            if (response.Status == CoordinatorStatus.Success)
            {
                _coordinator = response.Coordinator;
            }
            else
            {
                throw new Exception("Failed to create voting coordinator");
            }
        }

        public CoordinatorStatus RegisterBallot(RegisterBallotMessage message)
        {
            return Protect(_coordinator, () => CoordinatorApi.RegisterBallot(_coordinator, message));
        }

        public CoordinatorStatus CastBallot(BallotIdentifier ballotId)
        {
            return Protect(_coordinator, () => CoordinatorApi.CastBallot(_coordinator, ballotId));
        }

        public CoordinatorStatus SpoilBallot(BallotIdentifier ballotId)
        {
            return Protect(_coordinator, () => CoordinatorApi.SpoilBallot(_coordinator, ballotId));
            
        }

        public CoordinatorStatus ExportBallots(File file)
        {
            if (_coordinator == UIntPtr.Zero)
            {
                throw new NullReferenceException();
            }
            return CoordinatorApi.ExportBallots(_coordinator, file);
        }

        public void Dispose()
        {
            ProtectVoid(_coordinator, () => CoordinatorApi.FreeCoordinator(_coordinator));
        }
    }
}