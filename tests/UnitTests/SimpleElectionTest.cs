using System;
using System.Collections.Generic;
using ElectionGuard.SDK;
using ElectionGuard.SDK.Config;
using NUnit.Framework;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Decryption;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.Models;
using ElectionGuard.SDK.Voting;
using ElectionGuard.SDK.Voting.Encrypter;
using UnitTests.Mocks;

using VotingCoordinatorStatus = ElectionGuard.SDK.Voting.Coordinator.CoordinatorStatus;
using DecryptionCoordinatorStatus = ElectionGuard.SDK.Decryption.Coordinator.CoordinatorStatus;
using DecryptionTrusteeStatus = ElectionGuard.SDK.Decryption.Trustee.TrusteeStatus;

namespace UnitTests
{
    [TestFixture(3, 3, 1, 3)]
    public class SimpleElectionTest
    {
        private ElectionGuardConfig _electionGuardConfig;
        private readonly ElectionManifest _electionManifest;

        // Key Ceremony
        private const string KeyCeremonyStage = "Key Ceremony";
        private string _baseHashCode;
        private string _jointKey;
        private Dictionary<int, string> _trusteeKeys;
        private int _numberOfSelections;

        // Voting
        private const string VotingStage = "Voting";
        private CryptographyParameters _parameters;
        private VotingCoordinator _votingCoordinator;
        private List<VotingEncrypter> _votingEncrypters;
        private const string VotingResultsPrefix = "voting_result";
        private File _votingResultsFile;

        // Decryption
        private const string DecryptionStage = "Decryption";
        private DecryptionCoordinator _decryptionCoordinator;
        private List<DecryptionTrustee> _decryptionTrustees;
        private DecryptionFragmentsRequest[] _decryptionFragmentsRequests;
        private byte[] _decryptionRequestPresent;
        private const string TallyPrefix = "tally";

        public SimpleElectionTest(int numberOfTrustees, int threshold)
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
        }

        [Test, Order(1), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step01_InitializeElection()
        {
            var result = Election.CreateElection(_electionGuardConfig, _electionManifest);

            _jointKey = result.ElectionGuardConfig.JointPublicKey;
            Assert.That(string.IsNullOrEmpty(_jointKey), Is.False);

            _trusteeKeys = result.TrusteeKeys;
            Assert.AreEqual(result.ElectionGuardConfig.NumberOfTrustees, _trusteeKeys.Count);

            _numberOfSelections = result.ElectionGuardConfig.NumberOfSelections;
            Assert.Greater(_numberOfSelections, 0);
            var expectedNumberOfSelections = 3;
            Assert.AreEqual(expectedNumberOfSelections, _numberOfSelections);
        }

        //        [Test, Order(2), NonParallelizable]
        //        [Category(VotingStage)]
        //        public void Step02_VotingInitialization()
        //        {
        //            if (_numberOfEncrypters * _numberOfBallotsPerEncrypter > MaxValues.MaxBallots)
        //            {
        //                throw new Exception("Max Ballots Exceeded");
        //            }
        //            _parameters = new CryptographyParameters();
        //            _votingEncrypters = new List<VotingEncrypter>();
        //            _votingCoordinator = new VotingCoordinator(_numberOfSelections);
        //            for(var i = 0; i < _numberOfEncrypters; i++)
        //            {
        //                _votingEncrypters.Add(new VotingEncrypter(_jointKey, _numberOfSelections, _baseHashCode));
        //            }
        //            Assert.NotNull(_votingCoordinator);
        //            Assert.AreEqual(_numberOfEncrypters, _votingEncrypters.Count);
        //        }

        //        [Test, Order(3), NonParallelizable]
        //        [Category(VotingStage)]
        //        public void Step03_SimulateVoting()
        //        {
        //            foreach (var encrypter in _votingEncrypters)
        //            {
        //                for (var i = 0; i < _numberOfBallotsPerEncrypter; i++)
        //                {
        //                    var randomBallot = BallotGenerator.FillRandomBallot(_numberOfSelections);
        //                    TestContext.Out.WriteLine($"Ballot: {string.Join(",", randomBallot)}");
        //                    var encryptedBallotReturn = encrypter.EncryptBallot(randomBallot);
        //                    Assert.AreEqual(EncrypterStatus.Success, encryptedBallotReturn.Status);

        //                    var tracker = BallotTools.DisplayTracker(encryptedBallotReturn.Tracker);
        //                    Assert.IsNotEmpty(tracker);
        //                    TestContext.Out.WriteLine($"Tracker: {tracker}");

        //                    var registerStatus = _votingCoordinator.RegisterBallot(encryptedBallotReturn.Message);
        //                    Assert.AreEqual(VotingCoordinatorStatus.Success, registerStatus);

        //                    if (BallotGenerator.RandomBit())
        //                    {
        //                        var castBallotStatus = _votingCoordinator.CastBallot(encryptedBallotReturn.Id);
        //                        Assert.AreEqual(VotingCoordinatorStatus.Success, castBallotStatus);
        //                    }
        //                    else
        //                    {
        //                        var spoilBallotStatus = _votingCoordinator.SpoilBallot(encryptedBallotReturn.Id);
        //                        Assert.AreEqual(VotingCoordinatorStatus.Success, spoilBallotStatus);
        //                    }
        //                }
        //            }
        //        }

        //        [Test, Order(4), NonParallelizable]
        //        [Category(VotingStage)]
        //        public void Step04_ExportBallots()
        //        {
        //            _votingResultsFile = FileTools.CreateNewFile(VotingResultsPrefix);
        //            var exportStatus = _votingCoordinator.ExportBallots(_votingResultsFile);
        //            Assert.AreEqual(VotingCoordinatorStatus.Success, exportStatus);
        //        }

        //        [Test, Order(5), NonParallelizable]
        //        [Category(DecryptionStage)]
        //        public void Step05_InitializeDecryption()
        //        {
        //            _decryptionCoordinator = new DecryptionCoordinator(_numberOfTrustees, _threshold);
        //            _decryptionTrustees = new List<DecryptionTrustee>();
        //            foreach(var trusteeKey in _trusteeKeys.Values)
        //            {
        //                _decryptionTrustees.Add(new DecryptionTrustee(_numberOfTrustees, _threshold, _numberOfSelections, trusteeKey, _baseHashCode));
        //            }
        //            Assert.NotNull(_decryptionCoordinator);
        //            Assert.AreEqual(_numberOfTrustees, _decryptionTrustees.Count);
        //        }

        //        [Test, Order(6), NonParallelizable]
        //        [Category(DecryptionStage)]
        //        public void Step06_TallyVotingRecords()
        //        {
        //            foreach (var trustee in _decryptionTrustees)
        //            {
        //                FileTools.SeekFileToBeginning(_votingResultsFile);
        //                var tallyStatus = trustee.TallyVotingRecord(_votingResultsFile);
        //                Assert.AreEqual(DecryptionTrusteeStatus.Success, tallyStatus);
        //            }
        //            FileTools.CloseFile(_votingResultsFile);
        //        }

        //        [Test, Order(7), NonParallelizable]
        //        [Category(DecryptionStage)]
        //        public void Step07_DecryptTallyShares()
        //        {
        //            foreach (var trustee in _decryptionTrustees)
        //            {
        //                var computeShareReturn = trustee.ComputeShare();
        //                Assert.AreEqual(DecryptionTrusteeStatus.Success, computeShareReturn.Status);

        //                var receiveShareStatus = _decryptionCoordinator.ReceiveShare(computeShareReturn.Share);
        //                Assert.AreEqual(DecryptionCoordinatorStatus.Success, receiveShareStatus);
        //            }

        //            var allSharesReceivedReturn = _decryptionCoordinator.AllSharesReceived();
        //            Assert.AreEqual(DecryptionCoordinatorStatus.Success, allSharesReceivedReturn.Status);

        //            Assert.AreEqual(_numberOfTrustees, allSharesReceivedReturn.NumberOfTrustees);
        //            Assert.LessOrEqual(_numberOfTrustees, allSharesReceivedReturn.RequestPresent.Length);
        //            Assert.LessOrEqual(_numberOfTrustees, allSharesReceivedReturn.Requests.Length);

        //            _decryptionRequestPresent = allSharesReceivedReturn.RequestPresent;
        //            _decryptionFragmentsRequests = allSharesReceivedReturn.Requests;
        //        }

        //        [Test, Order(08), NonParallelizable]
        //        [Category(DecryptionStage)]
        //        public void Step08_DecryptTallyDecryptionFragments()
        //        {
        //            for(var i = 0; i < _decryptionTrustees.Count; i++)
        //            {
        //                if (DecryptionTools.TrusteeMustBePresent(_decryptionRequestPresent[i]))
        //                {
        //                    var computeFragmentsReturn = _decryptionTrustees[i].ComputeFragments(_decryptionFragmentsRequests[i]);
        //                    Assert.AreEqual(DecryptionTrusteeStatus.Success, computeFragmentsReturn.Status);

        //                    var receiveFragmentsStatus = _decryptionCoordinator.ReceiveFragments(computeFragmentsReturn.Fragments);
        //                    Assert.AreEqual(DecryptionCoordinatorStatus.Success, receiveFragmentsStatus);
        //                }
        //            }

        //            var tallyFile = FileTools.CreateNewFile(TallyPrefix);
        //            var allFragmentsReceivedStatus = _decryptionCoordinator.AllFragmentsReceived(tallyFile);
        //            Assert.AreEqual(DecryptionCoordinatorStatus.Success, allFragmentsReceivedStatus);
        //            FileTools.CloseFile(tallyFile);
        //        }

        //        [OneTimeTearDown]
        //        public void OneTimeTearDown()
        //        {
        //            _parameters?.Dispose();

        //            _votingCoordinator.Dispose();
        //            foreach (var encrypter in _votingEncrypters)
        //            {
        //                encrypter.Dispose();
        //            }

        //            _decryptionCoordinator.Dispose();
        //            foreach (var trustee in _decryptionTrustees)
        //            {
        //                trustee.Dispose();
        //            }
        //        }
    }
}