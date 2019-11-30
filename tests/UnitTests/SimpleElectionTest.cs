using ElectionGuard.SDK;
using ElectionGuard.SDK.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnitTests.Mocks;

namespace UnitTests
{
    [TestFixture(3, 3, 5, "exported_results", "test_ballots-")]
    public class SimpleElectionTest
    {
        private ElectionGuardConfig _electionGuardConfig;
        private readonly ElectionManifest _electionManifest;
        private readonly int _numberOfBallots;
        private readonly string _exportFolder;
        private readonly string _ballotsPrefix;

        // Key Ceremony
        private const string KeyCeremonyStage = "Key Ceremony";
        private IDictionary<int, string> _trusteeKeys;

        // Voting
        private const string VotingStage = "Voting";
        private ICollection<string> _encryptedBallots;
        private ICollection<long> _ballotIds;

        // Decryption
        private const string DecryptionStage = "Decryption";

        public SimpleElectionTest(int numberOfTrustees, int threshold, int numberOfBallots, string exportFolder, string ballotsPrefix)
        {
            _electionGuardConfig = new ElectionGuardConfig()
            {
                NumberOfTrustees = numberOfTrustees,
                Threshold = threshold,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };
            _electionManifest = new ElectionManifest()
            {
                Contests = new Contest[]{ new YesNoContest()
                {
                    Type = "YesNo"
                } },
            };

            if (!string.IsNullOrWhiteSpace(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            _exportFolder = exportFolder;
            _ballotsPrefix = ballotsPrefix;
            _numberOfBallots = numberOfBallots;
            _encryptedBallots = new List<string>();
            _ballotIds = new List<long>();
        }

        [Test, Order(1), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step01_InitializeElection()
        {
            var result = Election.CreateElection(_electionGuardConfig, _electionManifest);
            _electionGuardConfig = result.ElectionGuardConfig;

            Assert.That(string.IsNullOrEmpty(_electionGuardConfig.JointPublicKey), Is.False);

            _trusteeKeys = result.TrusteeKeys;
            Assert.AreEqual(result.ElectionGuardConfig.NumberOfTrustees, _trusteeKeys.Count);

            Assert.Greater(_electionGuardConfig.NumberOfSelections, 0);
            var expectedNumberOfSelections = 3;
            Assert.AreEqual(expectedNumberOfSelections, result.ElectionGuardConfig.NumberOfSelections);
        }

        [Test, Order(2), NonParallelizable]
        [Category(VotingStage)]
        public void Step02_VotingAndEncryption()
        {
            var currentNumBallots = 0;
            while (currentNumBallots < _numberOfBallots)
            {
                // generates new random ballot
                var randomBallot = BallotGenerator.FillRandomBallot(_electionGuardConfig.NumberOfSelections);

                var result = Voting.EncryptBallot(randomBallot, _electionGuardConfig, currentNumBallots);

                Assert.IsNotEmpty(result.EncryptedBallotMessage);
                Assert.IsNotEmpty(result.Tracker);
                Assert.AreEqual(result.Identifier, currentNumBallots);
                Assert.Greater(result.CurrentNumberOfBallots, currentNumBallots);

                _encryptedBallots.Add(result.EncryptedBallotMessage);
                _ballotIds.Add(result.Identifier);

                currentNumBallots = (int)result.CurrentNumberOfBallots;
            }
        }

        [Test, Order(3), NonParallelizable]
        [Category(VotingStage)]
        public void Step03_RecordBallots()
        {
            var castedIds = new List<long>();
            var spoiledIds = new List<long>();

            // randomly assign to cast or spoil lists
            foreach(var id in _ballotIds)
            {
                if (BallotGenerator.RandomBit())
                {
                    castedIds.Add(id);
                }
                else
                {
                    spoiledIds.Add(id);
                }
            }

            var success = Voting.RecordBallots(_electionGuardConfig,
                                               _encryptedBallots,
                                               castedIds,
                                               spoiledIds,
                                               _exportFolder,
                                               _ballotsPrefix);

            Assert.IsTrue(success);
        }
    }
}