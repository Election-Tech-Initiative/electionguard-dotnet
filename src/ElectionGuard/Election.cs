using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Models;
using ElectionGuard.SDK.Serialization;
using System;
using System.Collections.Generic;

namespace ElectionGuard.SDK
{
    public class Election
    {
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

            CreateElection(electionManifest);
            CalculateSelections(electionManifest);
        }

        public ElectionProfile Profile { get; set; }

        public BallotTrackerConfig BallotTrackerConfig { get; set; }

        public int NumberOfTrustees { get; }

        public int Threshold { get; }

        public string PublicJointKey { get; private set; }

        public Dictionary<int, string> TrusteeKeys { get; private set; }

        public int NumberOfSelections { get; private set; }

        private void CreateElection(ElectionManifest electionManifest)
        {
            var config = new APIConfig
            {
                NumberOfTrustees = (uint)NumberOfTrustees,
                Threshold = (uint)Threshold,
                SubgroupOrder = 0, // TODO: something out of the electionManifest?
                ElectionMetadata = "placeholder", // TODO: something out of the electionManifest?
            };

            // Set up trustee states array to be allocated in the api
            var trusteeStates = new SerializedBytes[MaxValues.MaxTrustees];

            // Call the C library API to create the election and get back the joint public key bytes
            // The trusteeStates array should be filled out with the appropriate serialized returns as well
            var jointPublicKey = API.CreateElection(config, trusteeStates);

            // Convert the joint public key to a base64 string that can be stored by a client
            PublicJointKey = ByteSerializer.ConvertToBase64String(jointPublicKey);

            // Iterate through the trusteeStates returned and convert each to its base64 reprentation
            for(var i = 0; i < trusteeStates.Length; i++)
            {
                var trusteeState = trusteeStates[i];
                if (trusteeState.Length > 0)
                {
                    var trusteeStateKeys = ByteSerializer.ConvertToBase64String(trusteeState);
                    TrusteeKeys.Add(i, trusteeStateKeys);
                }
            }

            // Free bytes in unmanaged memory
            API.FreeCreateElection(jointPublicKey, trusteeStates);
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
