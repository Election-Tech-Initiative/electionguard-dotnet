using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Voting.Messages;

namespace ElectionGuard.SDK.Voting.Tracker
{
    public static class TrackerApi
    {
        [DllImport("electionguard", EntryPoint = "display_ballot_tracker")]
        internal static extern IntPtr DisplayBallotTracker(BallotTracker tracker);
    }
}