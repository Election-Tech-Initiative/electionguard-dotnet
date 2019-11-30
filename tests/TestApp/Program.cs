using ElectionGuard.SDK;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;

/// <summary>
/// Simple console app to use as a playground for running the C# SDK methods
/// This is easier to run when we want to use gdb to debug into the C methods
/// that get called by the C# side.
/// </summary>
namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var initialConfig = new ElectionGuardConfig()
            {
                NumberOfTrustees = 3,
                Threshold = 3,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };
            var manifest = new ElectionManifest()
            {
                Contests = new Contest[]{ new YesNoContest()
                {
                    Type = "YesNo"
                } },
            };

            var electionResult = Election.CreateElection(initialConfig, manifest);

            // create predictable ballot selections (election.NumberOfSelections should 3 for a single YesNoContest)
            var selections = new bool[3] { false, true, false };
            var currentNumberOfBallots = 0;
            var encryptBallotResult = Voting.EncryptBallot(selections, electionResult.ElectionGuardConfig, currentNumberOfBallots);

            Console.WriteLine($"encryptedBallotResult.id = {encryptBallotResult.Identifier}");
            Console.WriteLine($"tracker = {encryptBallotResult.Tracker}");
            Console.WriteLine($"EncryptedBallotMessage.Length = {encryptBallotResult.EncryptedBallotMessage.Length}");
            Console.WriteLine($"CurrentNumberOfBallots = {encryptBallotResult.CurrentNumberOfBallots}");

            var encryptedBallotList = new List<string>();
            encryptedBallotList.Add(encryptBallotResult.EncryptedBallotMessage);

            var castedIds = new List<long> { 0 };
            var spoiledIds = new List<long> { };

            // does not pass in optional export path or prefix-filename
            // will output voting results to CWD with default prefix
            var recordResult = Voting.RecordBallots(electionResult.ElectionGuardConfig, encryptedBallotList, castedIds, spoiledIds);

            Console.WriteLine($"RecordBallots recordResult = {recordResult}");


        }
    }
}
