using ElectionGuard.SDK.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using ElectionGuard.SDK;
using UnitTests.Mocks;

namespace UnitTests
{
    [TestFixture(3, 3, 5, "exported_results", "encrypted-ballots","registered-ballots-", "test-tally-")]
    public class SimpleElectionTest
    {
        private ElectionGuardConfig _electionGuardConfig;
        private readonly int _numberOfBallots;
        private readonly string _exportFolder;
        private readonly string _encryptedBallotsPrefix;
        private readonly string _ballotsPrefix;
        private readonly string _tallyPrefix;
        private readonly int _expectedNumberOfSelected;

        // Key Ceremony
        private const string KeyCeremonyStage = "Key Ceremony";
        private IDictionary<int, string> _trusteeKeys;

        // Voting
        private const string VotingStage = "Voting";
        private readonly ICollection<string> _encryptedBallots;
        private readonly ICollection<string> _ballotIds;
        private string _ballotsFilename;

        // Decryption
        private const string DecryptionStage = "Decryption";

        public SimpleElectionTest(int numberOfTrustees, int threshold, int numberOfBallots,
            string exportFolder, string encryptedBallotsPrefix, string ballotsPrefix, string tallyPrefix)
        {
            _electionGuardConfig = new ElectionGuardConfig()
            {
                NumberOfTrustees = numberOfTrustees,
                NumberOfSelections = 3,
                Threshold = threshold,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };

            if (!string.IsNullOrWhiteSpace(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            _exportFolder = exportFolder;
            _encryptedBallotsPrefix = encryptedBallotsPrefix;
            _ballotsPrefix = ballotsPrefix;
            _tallyPrefix = tallyPrefix;
            _numberOfBallots = numberOfBallots;
            _encryptedBallots = new List<string>();
            _ballotIds = new List<string>();
            _expectedNumberOfSelected = 2;
        }

        [Test, Order(1), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step01_InitializeElection()
        {
            var result = ElectionGuardApi.CreateElection(_electionGuardConfig);
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
            var deleteSuccess = ElectionGuardApi.SoftDeleteEncryptedBallotsFile(_exportFolder, _encryptedBallotsPrefix);
            var currentNumBallots = 0;
            while (currentNumBallots < _numberOfBallots)
            {
                // generates new random ballot
                var randomBallot = BallotGenerator.FillRandomBallot(
                    _electionGuardConfig.NumberOfSelections, _expectedNumberOfSelected);

                var result = ElectionGuardApi.EncryptBallot(
                    randomBallot, 
                    _expectedNumberOfSelected, 
                    _electionGuardConfig, 
                    $"{currentNumBallots}",
                    _exportFolder,
                    _encryptedBallotsPrefix
                );

                Assert.IsNotEmpty(result.EncryptedBallotMessage);
                Assert.IsNotEmpty(result.Tracker);
                Assert.AreEqual(result.ExternalIdentifier, $"{currentNumBallots}");
                Assert.IsNotEmpty(result.OutputFileName);

                _encryptedBallots.Add(result.EncryptedBallotMessage);
                _ballotIds.Add(result.ExternalIdentifier);

                currentNumBallots++;
            }
        }

        [Test, Order(3), NonParallelizable]
        [Category(VotingStage)]
        public void Step03_RecordBallots()
        {
            var castIds = new List<string>();
            var spoiledIds = new List<string>();

            // randomly assign to cast or spoil lists
            foreach(var id in _ballotIds)
            {
                if (BallotGenerator.RandomBit())
                {
                    castIds.Add(id);
                }
                else
                {
                    spoiledIds.Add(id);
                }
            }

            var result = ElectionGuardApi.RecordBallots(
                _electionGuardConfig,
                castIds,
                spoiledIds,
                _ballotIds,
                _encryptedBallots,
                _exportFolder,
                _ballotsPrefix
            );

            Assert.AreEqual(castIds.Count, result.CastedBallotTrackers.Count);
            Assert.AreEqual(spoiledIds.Count, result.SpoiledBallotTrackers.Count);
            _ballotsFilename = result.EncryptedBallotsFilename;
            Assert.IsNotNull(_ballotsFilename);
        }

        [Test, Order(4), NonParallelizable]
        [Category(DecryptionStage)]
        public void Step04_TallyVotes()
        {
            // assume we have the equivalent number of trustees present to the threshold number required
            var numberOfTrusteesPresent = _electionGuardConfig.Threshold;
            var result = ElectionGuardApi.TallyVotes(
                _electionGuardConfig,
                _trusteeKeys.Values,
                numberOfTrusteesPresent,
                _ballotsFilename,
                _exportFolder,
                _tallyPrefix
            );

            Assert.IsNotNull(result.EncryptedTallyFilename);
            Assert.AreEqual(_electionGuardConfig.NumberOfSelections, result.TallyResults.Count);
        }
    }
}