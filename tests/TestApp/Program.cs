using ElectionGuard.SDK;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TestApp
{
    public class TestBallot
    {
        public string ExternalIdentifier { get; set; }
        public bool IsCast { get; set; }
        public bool IsSpoiled {get; set; }
        public string EncryptedBallotMessage { get; set; }
        public string Tracker { get; set; }
        public bool[] Selections { get; set; }
    }

    /// <summary>
    /// Simple console app to use as a playground for running the C# SDK methods
    /// This is easier to run when we want to use gdb to debug into the C methods
    /// that get called by the C# side.
    /// </summary>
    class Program
    {
        static int NUM_TRUSTEES = 3;
        static int THRESHOLD = 2;
        static int NUM_ENCRYPTERS = 3;
        static int NUM_SELECTIONS = 12;                 // the number of total contest selections for an election
        static int DECRYPTING_TRUSTEES = 2;             // must be >= THRESHOLD && <= NUM_TRUSTEES
        static int NUM_RANDOM_BALLOTS = 5;              // the number of ballots to use when executing the test

        static void Main(string[] args)
        {
            var initialConfig = new ElectionGuardConfig()
            {
                NumberOfTrustees = NUM_TRUSTEES,
                NumberOfSelections = NUM_SELECTIONS,
                Threshold = THRESHOLD,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };

            // Create Election

            Console.WriteLine("\n--- Create Election ---\n");

            var electionResult = ElectionGuardApi.CreateElection(initialConfig);

            foreach(KeyValuePair<int, string> entry in electionResult.TrusteeKeys)
            {
                if (String.IsNullOrWhiteSpace(entry.Value))
                {
                    throw new Exception("Error reading trustee keys");
                }
            }

            // creates a single predictable ballot selections (election.NumberOfSelections should 6 for 2 sets of YesNoContest)
            // var selections = new[]
            // {
            //     new[] { false, true, false, false, true, false },
            //     new[] { true, false, false, true, false, false },
            //     new[] { true, false, false, false, true, false },
            //     new[] { false, false, true, false, true, false }
            // };

            // Encrypt Ballots

            Console.WriteLine("\n--- Encrypt Ballots ---\n");

            var now = new DateTime();


            var expectedNumberOfSelected = 2;
            var encryptedBallotsFileName = "";
            var encryptedOutputPath = "./ballots_encrypter/";
            var encryptedOutputPrefix = $"encrypted-ballots_{now.Year}_{now.Month}_{now.Day}";

            if (!ElectionGuardApi.SoftDeleteEncryptedBallotsFile(encryptedOutputPath, encryptedOutputPrefix))
            {
                throw new Exception("Failed soft deleting the encrypted ballots file");
            }
        
            var testBallots = new List<TestBallot>();

            for (var i = 0; i < NUM_RANDOM_BALLOTS; i++)
            {
                var ballot = new TestBallot();
                ballot.ExternalIdentifier = $"{encryptedOutputPrefix}_{i}";
                ballot.Selections = FillRandomBallot(NUM_SELECTIONS, expectedNumberOfSelected);

                var encryptBallotResult = ElectionGuardApi.EncryptBallot(
                    ballot.Selections, 
                    expectedNumberOfSelected, 
                    electionResult.ElectionGuardConfig, 
                    ballot.ExternalIdentifier,
                    encryptedOutputPath,
                    encryptedOutputPrefix
                );
                
                Console.WriteLine($"Encrypted Ballot {i}:");
                Console.WriteLine($"\tIdentifier = {encryptBallotResult.ExternalIdentifier}");
                Console.WriteLine($"\tTracker = {encryptBallotResult.Tracker}");
                Console.WriteLine($"\tEncryptedBallotMessage.Length = {encryptBallotResult.EncryptedBallotMessage.Length}");

                ballot.EncryptedBallotMessage = encryptBallotResult.EncryptedBallotMessage;
                ballot.Tracker = encryptBallotResult.Tracker;
                encryptedBallotsFileName = encryptBallotResult.OutputFileName;

                testBallots.Add(ballot);
            }

             // TODO: test simulating multiple encrypters or an encrypter being reset

            // [START] OPTIONAL:

            // When running an encrypter on another device, it is possible to import a file
            // and pass it to the tally functions, but this is not strictly necessary
            // from an API Perspective.

            Console.WriteLine("\n--- Load Ballots ---\n");

            var loadBallotsResult = ElectionGuardApi.LoadBallotsFile(
                0, 
                NUM_RANDOM_BALLOTS, 
                NUM_SELECTIONS, 
                encryptedBallotsFileName
            );

            var loadedExternalIdentifiers = (List<string>)loadBallotsResult.ExternalIdentifiers;
            var loadedEncryptedBallots = (List<string>)loadBallotsResult.EncryptedBallotMessages;

            Debug.Assert(
                LoadedBallotIdentifiersMatchEncryptedBallots(
                    loadedExternalIdentifiers,
                    testBallots,
                    NUM_RANDOM_BALLOTS
                )
            );

            Debug.Assert(
                LoadedBallotsMatchEncryptedBallots(
                    loadedEncryptedBallots,
                    testBallots,
                    NUM_RANDOM_BALLOTS
                )
            );

            // TODO: test loading ballots in batches

            // [END] OPTIONAL:

            // Register & Record Cast/Spoil Multiple Ballots

            Console.WriteLine("\n--- Randomly Assigning Ballots to be Cast or Spoil Arrays ---\n");

            int currentCastIndex = 0;
            int currentSpoiledIndex = 0;
            var castIds = new List<string>();
            var spoiledIds = new List<string>();
            var memoryExternalIdentifiers = new List<string>();
            var memoryEncryptedBallots = new List<string>();

            for (int i = 0; i < NUM_RANDOM_BALLOTS; i++)
            {
                memoryExternalIdentifiers.Add(testBallots[i].ExternalIdentifier);
                memoryEncryptedBallots.Add(testBallots[i].EncryptedBallotMessage);

                if (RandomBit())
                {
                    testBallots[i].IsCast = true;
                    testBallots[i].IsSpoiled = false;
                    castIds.Add(testBallots[i].ExternalIdentifier);

                    Console.WriteLine($"Ballot Id: {testBallots[i].ExternalIdentifier} - Cast!");
                    currentCastIndex++;
                }
                else
                {
                    testBallots[i].IsCast = false;
                    testBallots[i].IsSpoiled = true;
                    spoiledIds.Add(testBallots[i].ExternalIdentifier);

                    Console.WriteLine($"Ballot Id: {testBallots[i].ExternalIdentifier} - Spiled!");
                    currentSpoiledIndex++;
                }
            }

            if ((currentCastIndex + currentSpoiledIndex) != NUM_RANDOM_BALLOTS)
            {
                throw new Exception("Cast and Spil did not match expected ballots");
            }

            Console.WriteLine("\n--- Record Ballots (Register, Cast, and Spoil) ---\n");

            var registeredBallotsOutputPath = "./ballots/";
            var registeredBallotsOutputPrefix = $"registered-ballots_{now.Year}_{now.Month}_{now.Day}";

            var recordResult = ElectionGuardApi.RecordBallots(
                electionResult.ElectionGuardConfig, 
                castIds, 
                spoiledIds, 
                memoryExternalIdentifiers,
                memoryEncryptedBallots,
                registeredBallotsOutputPath, 
                registeredBallotsOutputPrefix
            );

            var castedTrackers = (List<string>)recordResult.CastedBallotTrackers;
            var spoiledTrackers = (List<string>)recordResult.SpoiledBallotTrackers;

            Console.WriteLine($"RecordBallots cast trackers\n");
            for (int i = 0; i < currentCastIndex; i++)
            {
                Console.WriteLine($"\t{castIds[i]}: {castedTrackers[i]}");
            }

            Console.WriteLine($"\nRecordBallots spoiled trackers\n");
            for (int i = 0; i < currentSpoiledIndex; i++)
            {
                Console.WriteLine($"\t{spoiledIds[i]}: {spoiledTrackers[i]}");
            }

            Console.WriteLine("\nBallot registrations and recording of cast/spoil successful!\n");
            Console.WriteLine($"RecordBallots outputted to file = {recordResult.EncryptedBallotsFilename}");

            // Tally Votes & Decrypt Results

            Console.WriteLine("\n--- Tally & Decrypt Votes ---\n");

            var tallyOutputPath = "./tallies/";
            var tallyOutputPrefix = $"tally_{now.Year}_{now.Month}_{now.Day}";


            var tallyResult = ElectionGuardApi.TallyVotes(
                electionResult.ElectionGuardConfig, 
                electionResult.TrusteeKeys.Values, 
                DECRYPTING_TRUSTEES, 
                recordResult.EncryptedBallotsFilename, 
                tallyOutputPath, 
                tallyOutputPrefix
            );

            var tallyResults = (List<int>)tallyResult.TallyResults;

            Console.WriteLine("\nTally Results:");
            for (int i = 0; i < electionResult.ElectionGuardConfig.NumberOfSelections; i++)
            {
                Debug.Assert(
                    ResultEqualsExpectedSelections(
                        testBallots,
                        tallyResults[i],
                        i
                    )
                );
            }

            Console.WriteLine($"\nTallyVotes output to file = {tallyResult.EncryptedTallyFilename}");

            Console.WriteLine("\n--- Done! ---\n\n");
        }

        static bool RandomBit()
        {
            var random = new Random();
            return random.Next(2) == 0;
        }

        // Note: this method fills the expected number of selected in the ballot
        // but does not guarantee the values are distributed correctly over the contest selections
        static bool[] FillRandomBallot(int selections, int expectedNumberOfSelected)
        {
            var numberSelected = 0;
            var result = new bool[selections];

            for(int i = 0; i < selections; i++)
            {
                if (RandomBit() && numberSelected < expectedNumberOfSelected)
                {
                    result[i] = true;
                    numberSelected++;
                }
                else
                {
                    result[i] = false;
                }
            }

            return result;
        }

        static bool LoadedBallotIdentifiersMatchEncryptedBallots(
            List<string> loadedBallotIdentifiers, List<TestBallot> testBallots, int ballotCount)
        {
            Console.WriteLine($"verifying {ballotCount} ballot identifiers");

            bool ok = true;
            for (var i = 0; i < ballotCount && ok; i++)
            {
                Console.WriteLine(
                    $"compare string:\n - expect: {testBallots[i].ExternalIdentifier}\n -actual: {loadedBallotIdentifiers[i]}");
                ok = String.Equals(testBallots[i].ExternalIdentifier, loadedBallotIdentifiers[i]);
            }

            if (!ok)
            {
                Console.WriteLine("loaded ballot identifiers did not match the encrypted ballots!");
            }

            return ok;
        }

        static bool LoadedBallotsMatchEncryptedBallots(
            List<string> loadedBallotMessages, List<TestBallot> testBallots, int ballotCount)
        {
            Console.WriteLine($"verifying {ballotCount} ballots");

            bool ok = true;
            for (int i = 0; i < ballotCount && ok; i++)
            {
                ok = String.Equals(testBallots[i].EncryptedBallotMessage, loadedBallotMessages[i]);
            }

            if (!ok)
            {
                Console.WriteLine("loaded ballot identifiers did not match the encrypted ballots!");
            }
            return ok;
        }

        static bool ResultEqualsExpectedSelections(List<TestBallot> testBallots, int actualTally, int selectionIndex)
        {
            var expectedTally = 0;
            for (int i = 0; i < NUM_RANDOM_BALLOTS; i++)
            {
                if (testBallots[i].IsCast && testBallots[i].Selections[selectionIndex])
                {
                    expectedTally++;
                }
            }
            Console.WriteLine($"\tselection: {selectionIndex}: expected: {expectedTally} actual: {actualTally}");

            return expectedTally == actualTally;
        }
        
    }
}
