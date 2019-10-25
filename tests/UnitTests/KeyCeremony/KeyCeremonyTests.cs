using System.Collections.Generic;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.KeyCeremony.Coordinator;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.KeyCeremony.Trustee;
using ElectionGuard.SDK.StateManagement;
using NUnit.Framework;

namespace UnitTests.KeyCeremony
{
    [TestFixture(1u, 1u)]
    [TestFixture(2u, 2u)]
    [TestFixture(3u, 3u)]
    [TestFixture(4u, 4u)]
    [TestFixture(5u, 5u)]
    [TestFixture(6u, 6u)]
    public class KeyCeremonyTests
    {
        private readonly byte[] _baseHashCode = { 0,0xff,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };
        private readonly uint _numberOfTrustees;
        private readonly uint _threshold;
        private CryptographyParameters _parameters;
        private KeyCeremonyCoordinator _coordinator;
        private List<KeyCeremonyTrustee> _trustees;
        private AllKeysReceivedMessage _allKeysReceivedMessage;
        private AllSharesReceivedMessage _allSharesReceivedMessage;
        private List<TrusteeState> _trusteeStates;

        public KeyCeremonyTests(uint numberOfTrustees = 1, uint threshold = 1)
        {
            _numberOfTrustees = numberOfTrustees;
            _threshold = threshold;
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
        public void InitializeKeyCeremonyTest()
        {
            _coordinator = new KeyCeremonyCoordinator(_numberOfTrustees, _threshold);
            _trustees = new List<KeyCeremonyTrustee>();
            _trusteeStates = new List<TrusteeState>();
            for (uint i = 0; i < _numberOfTrustees; i++)
            {
                _trustees.Add(new KeyCeremonyTrustee(_numberOfTrustees, _threshold, i));
            }
            Assert.NotNull(_coordinator);
            Assert.AreEqual(_numberOfTrustees, _trustees.Count);
        }

        [Test, Order(2)]
        public void GenerateKeyTest()
        {
            var missingTrusteesKeysReturn = _coordinator.AllKeysReceived();
            Assert.AreEqual(CoordinatorStatus.MissingTrustees, missingTrusteesKeysReturn.Status);
            
            foreach (var trustee in _trustees)
            {
                var keyGeneratedReturn = trustee.GenerateKey(_baseHashCode);
                Assert.AreEqual(TrusteeStatus.Success, keyGeneratedReturn.Status);

                var keyReceivedStatus = _coordinator.ReceiveKey(keyGeneratedReturn.Message);
                Assert.AreEqual(CoordinatorStatus.Success, keyReceivedStatus);
            }

            var allKeysReceivedReturn = _coordinator.AllKeysReceived();
            Assert.AreEqual(CoordinatorStatus.Success, allKeysReceivedReturn.Status);

            _allKeysReceivedMessage = allKeysReceivedReturn.Message;
        }

        [Test, Order(3)]
        public void GenerateSharesTest()
        {
            var missingTrusteesSharesReturn = _coordinator.AllSharesReceived();
            Assert.AreEqual(CoordinatorStatus.MissingTrustees, missingTrusteesSharesReturn.Status);

            foreach (var trustee in _trustees)
            {
                var generateShareResponse = trustee.GenerateShares(_allKeysReceivedMessage);
                Assert.AreEqual(TrusteeStatus.Success, generateShareResponse.Status);

                var receiveShareStatus = _coordinator.ReceiveShares(generateShareResponse.Message);
                Assert.AreEqual(CoordinatorStatus.Success, receiveShareStatus);
            }

            var allSharesReceivedReturn = _coordinator.AllSharesReceived();
            Assert.AreEqual(CoordinatorStatus.Success, allSharesReceivedReturn.Status);

            _allSharesReceivedMessage = allSharesReceivedReturn.Message;
        }

        [Test, Order(4)]
        public void VerifySharesTest()
        {
            var missingTrusteesVerifiedSharesReturn = _coordinator.PublishJointKey();
            Assert.AreEqual(CoordinatorStatus.MissingTrustees, missingTrusteesVerifiedSharesReturn.Status);

            foreach (var trustee in _trustees)
            {
                var verifySharesReturn = trustee.VerifyShares(_allSharesReceivedMessage);
                Assert.AreEqual(TrusteeStatus.Success, verifySharesReturn.Status);

                var receivedVerifiedSharesStatus = _coordinator.ReceiveSharesVerification(verifySharesReturn.Message);
                Assert.AreEqual(CoordinatorStatus.Success, receivedVerifiedSharesStatus);
            }

            var publishJointKeyReturn = _coordinator.PublishJointKey();
            Assert.AreEqual(CoordinatorStatus.Success, publishJointKeyReturn.Status);
        }

        [Test, Order(5)]
        public void ExportStateTest()
        {
            foreach (var trustee in _trustees)
            {
                var exportStateReturn = trustee.ExportState();
                Assert.AreEqual(TrusteeStatus.Success, exportStateReturn.Status);

                _trusteeStates.Add(exportStateReturn.State);
            }
            Assert.AreEqual(_numberOfTrustees, _trusteeStates.Count);
        }

        [Test, Order(6)]
        public void KeyCeremonyDisposeTest()
        {
            _coordinator.Dispose();
            foreach (var trustee in _trustees)
            {
                trustee.Dispose();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _parameters.Dispose();
        }
    }
}