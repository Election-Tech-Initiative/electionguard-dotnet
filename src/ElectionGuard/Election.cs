using ElectionGuard.SDK.ElectionGuardAPI;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;

namespace ElectionGuard.SDK
{
    public static class Election
    {
        public static CreateElectionResult CreateElection(ElectionGuardConfig initialConfig, ElectionManifest electionManifest)
        {
            var apiConfig = initialConfig.GetApiConfig();
            apiConfig.NumberOfSelections = (uint)CalculateSelections(electionManifest);

            // Set up trustee states array to be allocated in the api
            var trusteeStates = new SerializedBytes[Constants.MaxTrustees];
            
            try
            {
                // Call the C library API to create the election and get back the joint public key bytes
                // The trusteeStates array should be filled out with the appropriate serialized returns as well
                var success = API.CreateElection(ref apiConfig, trusteeStates);

                if (!success)
                {
                    throw new Exception("ElectionGuardAPI.CreateElection failed");
                }

                ElectionGuardConfig electionGuardConfig = new ElectionGuardConfig(apiConfig);

                var trusteeKeys = new Dictionary<int, string>();
                // Iterate through the trusteeStates returned and convert each to its base64 reprentation
                for (var i = 0; i < trusteeStates.Length; i++)
                {
                    var trusteeState = trusteeStates[i];
                    if (trusteeState.Length > 0)
                    {
                        var trusteeStateKeys = ByteSerializer.ConvertToBase64String(trusteeState);
                        trusteeKeys.Add(i, trusteeStateKeys);
                    }
                }

                return new CreateElectionResult()
                {
                    ElectionGuardConfig = electionGuardConfig,
                    TrusteeKeys = trusteeKeys
                };
            }
            finally
            {
                // Free bytes in unmanaged memory
                API.FreeCreateElection(apiConfig.SerializedJointPublicKey, trusteeStates);
            }
        }

        public static int CalculateSelections(ElectionManifest electionManifest)
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
            if (numberOfSelections > Constants.MaxSelections)
            {
                throw new Exception("Max Selections Exceeded.");
            }
            if (numberOfSelections == 0)
            {
                throw new Exception("Election has no selections");
            }
            return numberOfSelections;
        }
    }
}
