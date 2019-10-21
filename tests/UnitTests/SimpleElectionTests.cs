using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.KeyCeremony.Trustee;
using ElectionGuard.SDK.StateManagement;
using ElectionGuard.SDK.Voting;
using ElectionGuard.SDK.Voting.Encrypter;
using NUnit.Framework;
using KeyCeremonyCoordinatorStatus = ElectionGuard.SDK.KeyCeremony.Coordinator.CoordinatorStatus;
using VotingCoordinatorStatus = ElectionGuard.SDK.Voting.Coordinator.CoordinatorStatus;

namespace UnitTests
{
    [TestFixture(5u, 5u, 1u, 5u, 3u)]
    [TestFixture(3u, 3u, 2u, 10u, 2u)]
    public class SimpleElectionTests
    {
        private readonly byte[] _baseHashCode = { 0, 0xff, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private readonly uint _numberOfTrustees;
        private readonly uint _threshold;
        private readonly uint _numberOfEncrypters;
        private readonly uint _numberOfSelections;
        private readonly uint _numberOfBallotsPerEncrypter;
        private CryptographyParameters _parameters;
        private KeyCeremonyCoordinator _keyCeremonyCoordinator;
        private List<KeyCeremonyTrustee> _keyCeremonyTrustees;
        private AllKeysReceivedMessage _allKeysReceivedMessage;
        private AllSharesReceivedMessage _allSharesReceivedMessage;
        private List<TrusteeState> _trusteeStates;
        private JointPublicKey _jointKey;
        private VotingCoordinator _votingCoordinator;
        private List<VotingEncrypter> _votingEncrypters;
        private const string KeyCeremonyStage = "Key Ceremony";
        private const string VotingStage = "Voting";
        private const string VotingResultsPrefix = "voting_result";
        private const string TallyPrefix = "tally";

        public SimpleElectionTests(
            uint numberOfTrustees = 1,
            uint threshold = 1,
            uint numberOfEncrypters = 1,
            uint numberOfSelections = 1,
            uint numberOfBallotsPerEncrypter = 1)
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
            if (_numberOfTrustees > MaxValues.MaxTrustees || _threshold > MaxValues.MaxTrustees)
            {
                Assert.Ignore("Max Trustees Exceeded. Tests Ignored");
            }
            _parameters = new CryptographyParameters();
        }

        [Test, Order(1)]
        [Category(KeyCeremonyStage)]
        public void InitializeKeyCeremonyTest()
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

        [Test, Order(2)]
        [Category(KeyCeremonyStage)]
        public void GenerateKeyTest()
        {
            var missingTrusteesKeysReturn = _keyCeremonyCoordinator.AllKeysReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesKeysReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var keyGeneratedReturn = trustee.GenerateKey(_baseHashCode);
                Assert.AreEqual(TrusteeStatus.Success, keyGeneratedReturn.Status);

                var keyReceivedStatus = _keyCeremonyCoordinator.ReceiveKey(keyGeneratedReturn.Message);
                Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, keyReceivedStatus);
            }

            var allKeysReceivedReturn = _keyCeremonyCoordinator.AllKeysReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, allKeysReceivedReturn.Status);

            _allKeysReceivedMessage = allKeysReceivedReturn.Message;
        }

        [Test, Order(3)]
        [Category(KeyCeremonyStage)]
        public void GenerateSharesTest()
        {
            var missingTrusteesSharesReturn = _keyCeremonyCoordinator.AllSharesReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesSharesReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var generateShareResponse = trustee.GenerateShares(_allKeysReceivedMessage);
                Assert.AreEqual(TrusteeStatus.Success, generateShareResponse.Status);

                var receiveShareStatus = _keyCeremonyCoordinator.ReceiveShares(generateShareResponse.Message);
                Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, receiveShareStatus);
            }

            var allSharesReceivedReturn = _keyCeremonyCoordinator.AllSharesReceived();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.Success, allSharesReceivedReturn.Status);

            _allSharesReceivedMessage = allSharesReceivedReturn.Message;
        }

        [Test, Order(4)]
        [Category(KeyCeremonyStage)]
        public void VerifySharesTest()
        {
            var missingTrusteesVerifiedSharesReturn = _keyCeremonyCoordinator.PublishJointKey();
            Assert.AreEqual(KeyCeremonyCoordinatorStatus.MissingTrustees, missingTrusteesVerifiedSharesReturn.Status);

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var verifySharesReturn = trustee.VerifyShares(_allSharesReceivedMessage);
                Assert.AreEqual(TrusteeStatus.Success, verifySharesReturn.Status);

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

        [Test, Order(5)]
        [Category(KeyCeremonyStage)]
        public void ExportStateTest()
        {
            foreach (var trustee in _keyCeremonyTrustees)
            {
                var exportStateReturn = trustee.ExportState();
                Assert.AreEqual(TrusteeStatus.Success, exportStateReturn.Status);

                _trusteeStates.Add(exportStateReturn.State);
                var state = new byte[exportStateReturn.State.Length];
                Marshal.Copy(exportStateReturn.State.Bytes, state, 0, (int)exportStateReturn.State.Length);
                TestContext.WriteLine($"Trustee State: {BitConverter.ToString(state)}");
            }
            Assert.AreEqual(_numberOfTrustees, _trusteeStates.Count);
        }

        [Test, Order(6)]
        [Category(KeyCeremonyStage)]
        public void KeyCeremonyDisposeTest()
        {
            _keyCeremonyCoordinator.Dispose();
            foreach (var trustee in _keyCeremonyTrustees)
            {
                trustee.Dispose();
            }
        }

        [Test, Order(7)]
        [Category(VotingStage)]
        public void VotingInitializationTest()
        {
            _votingEncrypters = new List<VotingEncrypter>();
            _votingCoordinator = new VotingCoordinator(_numberOfSelections);
            for(uint i = 0; i < _numberOfEncrypters; i++)
            {
                _votingEncrypters.Add(new VotingEncrypter(_jointKey, _numberOfSelections, _baseHashCode));
            }
        }

        [Test, Order(8)]
        [Category(VotingStage)]
        public void SimulateVotingTest()
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

        [Test, Order(9)]
        [Category(VotingStage)]
        public void ExportBallotsTest()
        {
            var filePointer = ExportTools.CreateNewFile(VotingResultsPrefix);
            var exportStatus = _votingCoordinator.ExportBallots(filePointer);
            Assert.AreEqual(VotingCoordinatorStatus.Success, exportStatus);
            
            ExportTools.CloseFile(filePointer);
        }

        [Test, Order(10)]
        [Category(VotingStage)]
        public void DisposeVotingTest()
        {
            _votingCoordinator.Dispose();
            foreach (var encrypter in _votingEncrypters)
            {
                encrypter.Dispose();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _parameters.Dispose();
        }
    }
}