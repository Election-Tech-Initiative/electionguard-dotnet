using System;
using System.Collections.Generic;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Models;

namespace ElectionGuard.SDK
{
    public class Election
    {
        private byte[] _bashHash;
        private readonly byte[] _testBaseHash = { 0, 0xff, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Election (int numberOfTrustees, int threshold, ElectionManifest electionManifest)
        {
            Profile = electionManifest;
            BallotTrackerConfig = electionManifest.BallotTrackerConfig ?? new BallotTrackerConfig()
            {
                TrackerType = DefaultTracker.Type,
                TrackerSiteDisplay = DefaultTracker.SiteDisplay,
                TrackerUrlTemplate = DefaultTracker.SiteDisplay,
            };
            NumberOfTrustees = numberOfTrustees;
            Threshold = threshold;
            PublicJointKey = null;
            TrusteeKeys = new Dictionary<int, string>();
            NumberOfSelections = 0;

            GenerateBaseHash();
            KeyCeremony();
            CalculateSelections(electionManifest);
        }

        public ElectionProfile Profile { get; set; }

        public BallotTrackerConfig BallotTrackerConfig { get; set; }

        public int NumberOfTrustees { get; }

        public int Threshold { get; }

        public string PublicJointKey { get; private set; }

        public Dictionary<int, string> TrusteeKeys { get; private set; }

        public int NumberOfSelections { get; private set; }

        public string BaseHashCode => Convert.ToBase64String(_testBaseHash);

        private void GenerateBaseHash()
        {
            // TODO Properly Generate Base Hash
            _bashHash = _testBaseHash;
        }

        private void KeyCeremony()
        {
            var keyCeremonyProcessor = new KeyCeremonyProcessor(NumberOfTrustees, Threshold, _bashHash);
            PublicJointKey = keyCeremonyProcessor.GeneratePublicJointKey();
            TrusteeKeys = keyCeremonyProcessor.GenerateTrusteeKeys();
        }

        private void CalculateSelections(ElectionManifest electionManifest)
        {
            var numberOfSelections = 0;
            foreach (var contest in electionManifest.Contests)
            {
                switch (contest.Type.AsContestType())
                {
                    case ContestType.YesNo:
                        numberOfSelections += 3; // Yes + No + Null Vote
                        break;
                    case ContestType.Candidate:
                        var candidateContest = (CandidateContest)contest;
                        numberOfSelections += candidateContest.Candidates.Length;
                        numberOfSelections += candidateContest.Seats; // Null Votes
                        if (candidateContest.AllowWriteIns)
                        {
                            numberOfSelections += 1 * candidateContest.Seats;
                        }
                        break;
                }
            }
            if (numberOfSelections > MaxValues.MaxSelections)
            {
                throw new Exception("Max Selections Exceeded.");
            }
            if (numberOfSelections == 0)
            {
                throw new Exception("Election has no selections");
            }
            NumberOfSelections = numberOfSelections;
        }
    }
}
