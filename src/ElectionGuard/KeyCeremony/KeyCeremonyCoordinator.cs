﻿using System;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.KeyCeremony.Coordinator;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.Serialization;
using ElectionGuard.SDK.Utility;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyCoordinator : SafePointer, IDisposable
    {
        private readonly UIntPtr _coordinator;

        public KeyCeremonyCoordinator(int numberOfTrustees, int threshold)
        {
            var response = CoordinatorApi.NewCoordinator(Convert.ToUInt32(numberOfTrustees), Convert.ToUInt32(threshold));
            if (response.Status == CoordinatorStatus.Success)
            {
                _coordinator = response.Coordinator;
            }
            else
            {
                throw new Exception("Failed to create key ceremony coordinator");
            }
        }

        public CoordinatorStatus ReceiveKey(KeyGeneratedMessage message)
        {
            return Protect(_coordinator, () => CoordinatorApi.ReceiveKeyGenerated(_coordinator, message));
        }

        public AllKeysReceivedReturn AllKeysReceived()
        {
            return Protect(_coordinator, () => CoordinatorApi.AllKeysReceived(_coordinator));
        }

        public CoordinatorStatus ReceiveShares(SharesGeneratedMessage message)
        {
            return Protect(_coordinator, () => CoordinatorApi.ReceiveSharesGenerated(_coordinator, message));
        }

        public AllSharesReceivedReturn AllSharesReceived()
        {
            return Protect(_coordinator, () => CoordinatorApi.AllSharesReceived(_coordinator));
        }

        public CoordinatorStatus ReceiveSharesVerification(SharesVerifiedMessage message)
        {
            return Protect(_coordinator, () => CoordinatorApi.ReceiveSharesVerified(_coordinator, message));
        }

        public PublishJointKeyReturn PublishJointKey()
        {
            return Protect(_coordinator, () => CoordinatorApi.PublishJointKey(_coordinator));
        }

        public JointPublicKeyResponse GetPublishedJointKey(PublishJointKeyReturn publishJointKeyReturn)
        {
            if (publishJointKeyReturn.Status != CoordinatorStatus.Success)
            {
                throw new Exception("Failed to get published joint key");
            }
            var key = CoordinatorApi.GetPublishedJointKey(publishJointKeyReturn);
            return JointKeySerializer.Deserialize(key);
        }

        public void Dispose()
        {
            ProtectVoid(_coordinator, () => CoordinatorApi.FreeCoordinator(_coordinator));
        }
    }
}