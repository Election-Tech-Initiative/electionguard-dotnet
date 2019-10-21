using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Voting.Messages;
using ElectionGuard.SDK.IO;

namespace ElectionGuard.SDK.Voting.Coordinator
{
    internal static class CoordinatorApi
    {
        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_new")]
        internal static extern NewCoordinatorReturn NewCoordinator(uint numberOfSelections);

        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_free")]
        internal static extern void FreeCoordinator(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_register_ballot")]
        internal static extern CoordinatorStatus RegisterBallot(UIntPtr coordinator, RegisterBallotMessage message);

        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_cast_ballot")]
        internal static extern CoordinatorStatus CastBallot(UIntPtr coordinator, BallotIdentifier ballotId);

        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_spoil_ballot")]
        internal static extern CoordinatorStatus SpoilBallot(UIntPtr coordinator, BallotIdentifier ballotId);

        [DllImport("electionguard", EntryPoint = "Voting_Coordinator_export_ballots")]
        internal static extern CoordinatorStatus ExportBallots(UIntPtr coordinator, ref File file);
    }
}