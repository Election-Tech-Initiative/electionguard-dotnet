using System;
using System.Collections.Generic;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;

namespace ElectionGuard.SDK.KeyCeremony
{
    internal class KeyCeremonyProcessor : IDisposable
    {
        private readonly byte[] _bashHash;
        private readonly KeyCeremonyCoordinator _keyCeremonyCoordinator;
        private readonly List<KeyCeremonyTrustee> _keyCeremonyTrustees;
        private readonly CryptographyParameters _parameters;

        public KeyCeremonyProcessor(int numberOfTrustees, int threshold, byte[] bashHash)
        {
            if (numberOfTrustees > MaxValues.MaxTrustees || threshold > MaxValues.MaxTrustees)
            {
                // TODO Create Custom Exceptions
                throw new Exception($"Max Trustees of {MaxValues.MaxTrustees} Exceeded.");
            }

            if (numberOfTrustees > threshold)
            {
                throw new Exception($"Threshold of {threshold} exceeds number of trustees of ${numberOfTrustees}.");
            }

            _bashHash = bashHash;
            _parameters = new CryptographyParameters();
            _keyCeremonyCoordinator = new KeyCeremonyCoordinator(numberOfTrustees, threshold);
            _keyCeremonyTrustees = new List<KeyCeremonyTrustee>();
            for (var i = 0; i < numberOfTrustees; i++)
            {
                _keyCeremonyTrustees.Add(new KeyCeremonyTrustee(numberOfTrustees, threshold, i));
            }
        }

        public string GeneratePublicJointKey()
        {
            foreach (var trustee in _keyCeremonyTrustees)
            {
                var keyGeneratedReturn = trustee.GenerateKey(_bashHash);
                var keyReceivedStatus = _keyCeremonyCoordinator.ReceiveKey(keyGeneratedReturn.Message);
            }
            var allKeysReceivedReturn = _keyCeremonyCoordinator.AllKeysReceived();

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var generateShareResponse = trustee.GenerateShares(allKeysReceivedReturn.Message);
                var receiveShareStatus = _keyCeremonyCoordinator.ReceiveShares(generateShareResponse.Message);
            }

            var allSharesReceivedReturn = _keyCeremonyCoordinator.AllSharesReceived();

            foreach (var trustee in _keyCeremonyTrustees)
            {
                var verifySharesReturn = trustee.VerifyShares(allSharesReceivedReturn.Message);
                var receivedVerifiedSharesStatus = _keyCeremonyCoordinator.ReceiveSharesVerification(verifySharesReturn.Message);
            }

            var publishJointKeyReturn = _keyCeremonyCoordinator.PublishJointKey();
            var jointKey = _keyCeremonyCoordinator.GetPublishedJointKey(publishJointKeyReturn);
            return jointKey.Base64;
        }

        public Dictionary<int, string> GenerateTrusteeKeys()
        {
            var trusteeKeys = new Dictionary<int, string>();
            foreach (var trustee in _keyCeremonyTrustees)
            {
                var exportStateReturn = trustee.ExportState();
                var state = trustee.GetExportedState(exportStateReturn);
                trusteeKeys.Add((int)state.Index, state.Base64);
            }
            return trusteeKeys;
        }

        public void Dispose()
        {
            _parameters.Dispose();
            _keyCeremonyCoordinator.Dispose();
            foreach (var trustee in _keyCeremonyTrustees)
            {
                trustee.Dispose();
            }
        }
    }
}
