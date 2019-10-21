using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Voting.Messages;
using ElectionGuard.SDK.Voting.Tracker;

namespace ElectionGuard.SDK.Voting
{
    public struct BallotTools
    {
        public static string DisplayTracker(BallotTracker tracker)
        {
            if (tracker.Bytes == UIntPtr.Zero)
            {
                return string.Empty;
            }
            var trackerPointer = TrackerApi.DisplayBallotTracker(tracker);
            return Marshal.PtrToStringAnsi(trackerPointer);
        }
    }
}
