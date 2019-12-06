using ElectionGuard.SDK;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;

namespace TestApp
{
    /// <summary>
    /// Simple console app to use as a playground for running the C# SDK methods
    /// This is easier to run when we want to use gdb to debug into the C methods
    /// that get called by the C# side.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var initialConfig = new ElectionGuardConfig()
            {
                NumberOfTrustees = 3,
                NumberOfSelections = 6,
                Threshold = 3,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };

            var electionResult = ElectionGuardApi.CreateElection(initialConfig);

            // creates a single predictable ballot selections (election.NumberOfSelections should 6 for 2 sets of YesNoContest)
            var selections = new[]
            {
                new[] { false, true, false, false, true, false },
                new[] { true, false, false, true, false, false },
                new[] { true, false, false, false, true, false },
                new[] { false, false, true, false, true, false }
            };

            var expectedNumberOfSelected = 2; // we have 2 true values in each of the selection arrays above
            var currentNumberOfBallots = 0;
            var encryptedBallotList = new List<string>();
            for (var i = 0; i < selections.Length; i++)
            { 
                var encryptBallotResult = ElectionGuardApi.EncryptBallot(selections[i], expectedNumberOfSelected, electionResult.ElectionGuardConfig, currentNumberOfBallots);
                
                currentNumberOfBallots = (int)encryptBallotResult.CurrentNumberOfBallots;
                Console.WriteLine($"Encrypted Ballot {i}:");
                Console.WriteLine($"\tIdentifier = {encryptBallotResult.Identifier}");
                Console.WriteLine($"\tTracker = {encryptBallotResult.Tracker}");
                Console.WriteLine($"\tEncryptedBallotMessage.Length = {encryptBallotResult.EncryptedBallotMessage.Length}");
                Console.WriteLine($"\tCurrentNumberOfBallots = {encryptBallotResult.CurrentNumberOfBallots}");

                encryptedBallotList.Add(encryptBallotResult.EncryptedBallotMessage);
            }

            var castIds = new List<long> { 0, 3 };
            var spoiledIds = new List<long> { 1, 2 };

            // does not pass in optional export path or prefix-filename
            // will output voting results to CWD with default prefix
            var recordResult = ElectionGuardApi.RecordBallots(electionResult.ElectionGuardConfig, encryptedBallotList, castIds, spoiledIds);

            Console.WriteLine($"RecordBallots cast trackers");
            foreach (var cast in recordResult.CastedBallotTrackers) {
                Console.WriteLine($"\t{cast}");
            }
            Console.WriteLine($"RecordBallots spoiled trackers");
            foreach (var spoiled in recordResult.SpoiledBallotTrackers)
            {
                Console.WriteLine($"\t{spoiled}");
            }
            Console.WriteLine($"RecordBallots outputted to file = {recordResult.EncryptedBallotsFilename}");

            // assume number of trustees present is the threshold
            var numberOfTrusteesPresent = electionResult.ElectionGuardConfig.Threshold;
            // does not pass in optional export path or prefix-filename
            // will output voting results to CWD with default prefix
            var tallyResult = ElectionGuardApi.TallyVotes(electionResult.ElectionGuardConfig, electionResult.TrusteeKeys.Values, numberOfTrusteesPresent, recordResult.EncryptedBallotsFilename);

            var tallyIndex = 0;
            Console.WriteLine("Tally Results:");
            foreach(var tally in tallyResult.TallyResults)
            {
                Console.WriteLine($"\t{tallyIndex}: {tally}");
                tallyIndex++;
            }
            Console.WriteLine($"TallyVotes output to file = {tallyResult.EncryptedTallyFilename}");
        }
    }
}
