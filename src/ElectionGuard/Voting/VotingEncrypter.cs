using System;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Serialization;
using ElectionGuard.SDK.Utility;
using ElectionGuard.SDK.Voting.Encrypter;

namespace ElectionGuard.SDK.Voting
{
    public class VotingEncrypter : SafePointer, IDisposable
    {
        private UIntPtr _encrypter;
        private UniqueIdentifier _uniqueIdentifier;

        public VotingEncrypter(string jointKey, int numberOfSelections, string baseHashCode)
        {
            var byteHash = Convert.FromBase64String(baseHashCode);
            var jointPublicKey = EncrypterApi.NewJointPublicKey(jointKey);
            Initialize(jointPublicKey, numberOfSelections, byteHash);
            EncrypterApi.FreeJointPublicKey(jointPublicKey);
        }

        public VotingEncrypter(JointPublicKeyResponse jointKeyResponse, int numberOfSelections, byte[] byteHash)
        {
            var jointPublicKey = EncrypterApi.NewJointPublicKey(jointKeyResponse);
            Initialize(jointPublicKey, numberOfSelections, byteHash);
            EncrypterApi.FreeJointPublicKey(jointPublicKey);
        }

        public VotingEncrypter(JointPublicKey jointPublicKey, int numberOfSelections, byte[] byteHash)
        {
           Initialize(jointPublicKey, numberOfSelections, byteHash);
        }

        private void Initialize(JointPublicKey jointPublicKey, int numberOfSelections, byte[] byteHash)
        {
            if (byteHash.Length > CryptographySettings.HashDigestSizeBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(byteHash));
            }
            _uniqueIdentifier = EncrypterApi.NewUniqueIdentifier();
            var response = EncrypterApi.NewEncrypter(_uniqueIdentifier, jointPublicKey, Convert.ToUInt32(numberOfSelections), byteHash);

            if (response.Status == EncrypterStatus.Success)
            {
                _encrypter = response.Encrypter;
            }
            else
            {
                throw new Exception("Failed to create voting encrypter");
            }
        }

        public EncryptBallotReturn EncryptBallot(bool[] selections)
        {
            return Protect(_encrypter, () => EncrypterApi.EncryptBallot(_encrypter, selections));
        }


        public void Dispose()
        {
            ProtectVoid(_encrypter, () => EncrypterApi.FreeEncrypter(_encrypter));
            ProtectVoid(_uniqueIdentifier.Bytes, () => EncrypterApi.FreeUniqueIdentifier(_uniqueIdentifier.Bytes));
        }
    }
}