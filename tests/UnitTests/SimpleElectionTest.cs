using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Decryption;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.StateManagement;
using ElectionGuard.SDK.Voting;
using ElectionGuard.SDK.Voting.Encrypter;
using UnitTests.Mocks;

using KeyCeremonyCoordinatorStatus = ElectionGuard.SDK.KeyCeremony.Coordinator.CoordinatorStatus;
using VotingCoordinatorStatus = ElectionGuard.SDK.Voting.Coordinator.CoordinatorStatus;
using DecryptionCoordinatorStatus = ElectionGuard.SDK.Decryption.Coordinator.CoordinatorStatus;
using KeyCeremonyTrusteeStatus = ElectionGuard.SDK.KeyCeremony.Trustee.TrusteeStatus;
using DecryptionTrusteeStatus = ElectionGuard.SDK.Decryption.Trustee.TrusteeStatus;

namespace UnitTests
{
    [TestFixture(3u, 3u, 1u, 3u, 3u)]
    public class SimpleElectionTest
    {
        private readonly byte[] _baseHashCode = { 0, 0xff, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly uint _numberOfTrustees;
        private readonly uint _threshold;
        private readonly uint _numberOfEncrypters;
        private readonly uint _numberOfSelections;
        private readonly uint _numberOfBallotsPerEncrypter;
        private CryptographyParameters _parameters;

        // Key Ceremony
        private const string KeyCeremonyStage = "Key Ceremony";
        private KeyCeremonyCoordinator _keyCeremonyCoordinator;
        private List<KeyCeremonyTrustee> _keyCeremonyTrustees;
        private AllKeysReceivedMessage _allKeysReceivedMessage;
        private AllSharesReceivedMessage _allSharesReceivedMessage;
        private List<TrusteeState> _trusteeStates;
        private JointPublicKey _jointKey;

        // Voting
        private const string VotingStage = "Voting";
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

        public SimpleElectionTest(
            uint numberOfTrustees,
            uint threshold,
            uint numberOfEncrypters,
            uint numberOfSelections,
            uint numberOfBallotsPerEncrypter)
        {
            _numberOfTrustees = numberOfTrustees;
            _threshold = threshold;
            _numberOfEncrypters = numberOfEncrypters;
            _numberOfSelections = numberOfSelections;
            _numberOfBallotsPerEncrypter = numberOfBallotsPerEncrypter;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _parameters = new CryptographyParameters();

            if (_numberOfTrustees > MaxValues.MaxTrustees || _threshold > MaxValues.MaxTrustees)
            {
                Assert.Ignore("Max Trustees Exceeded. Tests Ignored");
            }
            if (_numberOfSelections > MaxValues.MaxSelections)
            {
                Assert.Ignore("Max Selections Exceeded. Tests Ignored");
            }
            if (_numberOfBallotsPerEncrypter * _numberOfEncrypters > MaxValues.MaxBallots)
            {
                Assert.Ignore("Max Ballots Exceeded. Tests Ignored");
            }
        }

        [Test, Order(1), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step01_InitializeKeyCeremony()
        {
            _keyCeremonyCoordinator = new KeyCeremonyCoordinator(_numberOfTrustees, _threshold);
            _keyCeremonyTrustees = new List<KeyCeremonyTrustee>();
            _trusteeStates = new List<TrusteeState>();
            for (uint i = 0; i < _numberOfTrustees; i++)
            {
                _keyCeremonyTrustees.Add(new KeyCeremonyTrustee(_numberOfTrustees, _threshold, i));
            }
            Assert.NotNull(_keyCeremonyCoordinator);
            Assert.AreEqual(_numberOfTrustees, _keyCeremonyTrustees.Count);
        }

        [Test, Order(2), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step02_GenerateKey()
        {
            var missingTrusteesKeysReturn = _keyCeremonyCoordinator.AllKeysReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesKeysReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var keyGeneratedReturn = trustee.GenerateKey(_baseHashCode);
                Assert.AreEqual(KeyCeremonyTrusteeStatus.Success, keyGeneratedReturn.Status);

                var keyReceivedStatus = _keyCeremonyCoordinator.ReceiveKey(keyGeneratedReturn.Message);
                Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, keyReceivedStatus);
            }

            var allKeysReceivedReturn = _keyCeremonyCoordinator.AllKeysReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, allKeysReceivedReturn.Status);

            _allKeysReceivedMessage = allKeysReceivedReturn.Message;
        }

        [Test, Order(3), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step03_GenerateShares()
        {
            var missingTrusteesSharesReturn = _keyCeremonyCoordinator.AllSharesReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesSharesReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var generateShareResponse = trustee.GenerateShares(_allKeysReceivedMessage);
                Assert.AreEqual(KeyCeremonyTrusteeStatus.Success, generateShareResponse.Status);

                var receiveShareStatus = _keyCeremonyCoordinator.ReceiveShares(generateShareResponse.Message);
                Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, receiveShareStatus);
            }

            var allSharesReceivedReturn = _keyCeremonyCoordinator.AllSharesReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, allSharesReceivedReturn.Status);

            _allSharesReceivedMessage = allSharesReceivedReturn.Message;
        }

        [Test, Order(4), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step04_VerifyShares()
        {
            var missingTrusteesVerifiedSharesReturn = _keyCeremonyCoordinator.PublishJointKey();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesVerifiedSharesReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var verifySharesReturn = trustee.VerifyShares(_allSharesReceivedMessage);
                Assert.AreEqual(KeyCeremonyTrusteeStatus.Success, verifySharesReturn.Status);

                var receivedVerifiedSharesStatus = _keyCeremonyCoordinator.ReceiveSharesVerification(verifySharesReturn.Message);
                Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, receivedVerifiedSharesStatus);
            }

            var publishJointKeyReturn = _keyCeremonyCoordinator.PublishJointKey();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, publishJointKeyReturn.Status);

            _jointKey = publishJointKeyReturn.Key;
            var jointKey = new byte[_jointKey.Length];
            Marshal.Copy(_jointKey.Bytes, jointKey, 0, (int)_jointKey.Length);
            TestContext.WriteLine($"Joint Key: {BitConverter.ToString(jointKey)}");
        }

        [Test, Order(5), NonParallelizable]
        [Category(KeyCeremonyStage)]
        public void Step05_ExportState()
        {
            foreach (var trustee in _keyCeremonyTrustees)
            {
                var exportStateReturn = trustee.ExportState();
                Assert.AreEqual(KeyCeremonyTrusteeStatus.Success, exportStateReturn.Status);

                _trusteeStates.Add(exportStateReturn.State);
                var state = new byte[exportStateReturn.State.Length];
                Marshal.Copy(exportStateReturn.State.Bytes, state, 0, (int)exportStateReturn.State.Length);
                TestContext.WriteLine($"Trustee State: {BitConverter.ToString(state)}");
            }
            Assert.AreEqual(_numberOfTrustees, _trusteeStates.Count);
        }

        [Test, Order(6), NonParallelizable]
        [Category(VotingStage)]
        public void Step06_VotingInitialization()
        {
            _votingEncrypters = new List<VotingEncrypter>();
            _votingCoordinator = new VotingCoordinator(_numberOfSelections);
            for(uint i = 0; i < _numberOfEncrypters; i++)
            {
                _votingEncrypters.Add(new VotingEncrypter(_jointKey, _numberOfSelections, _baseHashCode));
            }
            Assert.NotNull(_votingCoordinator);
            Assert.AreEqual(_numberOfEncrypters, _votingEncrypters.Count);
        }

        [Test, Order(7), NonParallelizable]
        [Category(VotingStage)]
        public void Step07_SimulateVoting()
        {
            foreach (var encrypter in _votingEncrypters)
            {
                for (uint i = 0; i < _numberOfBallotsPerEncrypter; i++)
                {
                    var randomBallot = BallotGenerator.FillRandomBallot(_numberOfSelections);
                    TestContext.Out.WriteLine($"Ballot: {string.Join(",", randomBallot)}");
                    var encryptedBallotReturn = encrypter.EncryptBallot(randomBallot);
                    Assert.AreEqual(EncrypterStatus.Success, encryptedBallotReturn.Status);

                    var tracker = BallotTools.DisplayTracker(encryptedBallotReturn.Tracker);
                    Assert.IsNotEmpty(tracker);
                    TestContext.Out.WriteLine($"Tracker: {tracker}");

                    var registerStatus = _votingCoordinator.RegisterBallot(encryptedBallotReturn.Message);
                    Assert.AreEqual(VotingCoordinatorStatus.Success, registerStatus);

                    if (BallotGenerator.RandomBit())
                    {
                        var castBallotStatus = _votingCoordinator.CastBallot(encryptedBallotReturn.Id);
                        Assert.AreEqual(VotingCoordinatorStatus.Success, castBallotStatus);
                    }
                    else
                    {
                        var spoilBallotStatus = _votingCoordinator.SpoilBallot(encryptedBallotReturn.Id);
                        Assert.AreEqual(VotingCoordinatorStatus.Success, spoilBallotStatus);
                    }
                }
            }
        }

        [Test, Order(8), NonParallelizable]
        [Ignore("Ignore until file format confirmed")]
        [Category(VotingStage)]
        public void Step08_ExportBallots()
        {
            _votingResultsFile = FileTools.CreateNewFile(VotingResultsPrefix);
            var exportStatus = _votingCoordinator.ExportBallots(_votingResultsFile);
            Assert.AreEqual(VotingCoordinatorStatus.Success, exportStatus);
        }

        [Test, Order(9), NonParallelizable]
        [Category(DecryptionStage)]
        public void Step09_InitializeDecryption()
        {
            _decryptionCoordinator = new DecryptionCoordinator(_numberOfTrustees, _threshold);
            _decryptionTrustees = new List<DecryptionTrustee>();
            foreach(var trusteeState in _trusteeStates)
            {
                _decryptionTrustees.Add(new DecryptionTrustee(_numberOfTrustees, _threshold, _numberOfSelections, trusteeState, _baseHashCode));
            }
            Assert.NotNull(_decryptionCoordinator);
            Assert.AreEqual(_numberOfTrustees, _decryptionTrustees.Count);
        }

        [Test, Order(10), NonParallelizable]
        [Ignore("Ignore until file format confirmed")]
        [Category(DecryptionStage)]
        public void Step10_TallyVotingRecords()
        {
            foreach (var trustee in _decryptionTrustees)
            {
                FileTools.SeekFileToBeginning(_votingResultsFile);
                var tallyStatus = trustee.TallyVotingRecord(_votingResultsFile);
                Assert.AreEqual(DecryptionTrusteeStatus.Success, tallyStatus);
            }
            FileTools.CloseFile(_votingResultsFile);
        }

        [Test, Order(11), NonParallelizable]
        [Ignore("Ignore until file format confirmed")]
        [Category(DecryptionStage)]
        public void Step11_DecryptTallyShares()
        {
            foreach (var trustee in _decryptionTrustees)
            {
                var computeShareReturn = trustee.ComputeShare();
                Assert.AreEqual(DecryptionTrusteeStatus.Success, computeShareReturn.Status);

                var receiveShareStatus = _decryptionCoordinator.ReceiveShare(computeShareReturn.Share);
                Assert.AreEqual(DecryptionCoordinatorStatus.Success, receiveShareStatus);
            }

            var allSharesReceivedReturn = _decryptionCoordinator.AllSharesReceived();
            Assert.AreEqual(DecryptionCoordinatorStatus.Success, allSharesReceivedReturn.Status);

            Assert.AreEqual(_numberOfTrustees, allSharesReceivedReturn.NumberOfTrustees);
            Assert.LessOrEqual(_numberOfTrustees, allSharesReceivedReturn.RequestPresent.Length);
            Assert.LessOrEqual(_numberOfTrustees, allSharesReceivedReturn.Requests.Length);

            _decryptionRequestPresent = allSharesReceivedReturn.RequestPresent;
            _decryptionFragmentsRequests = allSharesReceivedReturn.Requests;
        }

        [Test, Order(12), NonParallelizable]
        [Ignore("Ignore until file format confirmed")]
        [Category(DecryptionStage)]
        public void Step12_DecryptTallyDecryptionFragments()
        {
            for(var i = 0; i < _decryptionTrustees.Count; i++)
            {
                if (DecryptionTools.TrusteeMustBePresent(_decryptionRequestPresent[i]))
                {
                    var computeFragmentsReturn = _decryptionTrustees[i].ComputeFragments(_decryptionFragmentsRequests[i]);
                    Assert.AreEqual(DecryptionTrusteeStatus.Success, computeFragmentsReturn.Status);

                    var receiveFragmentsStatus = _decryptionCoordinator.ReceiveFragments(computeFragmentsReturn.Fragments);
                    Assert.AreEqual(DecryptionCoordinatorStatus.Success, receiveFragmentsStatus);
                }
            }

            var tallyFile = FileTools.CreateNewFile(TallyPrefix);
            var allFragmentsReceivedStatus = _decryptionCoordinator.AllFragmentsReceived(tallyFile);
            Assert.AreEqual(DecryptionCoordinatorStatus.Success, allFragmentsReceivedStatus);
            FileTools.CloseFile(tallyFile);
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _parameters.Dispose();

            _keyCeremonyCoordinator.Dispose();
            foreach (var trustee in _keyCeremonyTrustees)
            {
                trustee.Dispose();
            }

            _votingCoordinator.Dispose();
            foreach (var encrypter in _votingEncrypters)
            {
                encrypter.Dispose();
            }

            _decryptionCoordinator.Dispose();
            foreach (var trustee in _decryptionTrustees)
            {
                trustee.Dispose();
            }
        }
    }
}