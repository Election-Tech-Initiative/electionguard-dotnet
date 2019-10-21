using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Utility;
using ElectionGuard.SDK.Voting.Encrypter;

namespace ElectionGuard.SDK.Voting
{
    public class VotingEncrypter : SafePointer, IDisposable
    {
        private readonly UIntPtr _encrypter;
        private readonly UniqueIdentifier _uniqueIdentifier;

        public VotingEncrypter(JointPublicKey jointKey, uint numberOfSelections, byte[] byteHash)
        {
            if (byteHash.Length > CryptographySettings.HashDigestSizeBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(byteHash));
            }

            _uniqueIdentifier = EncrypterApi.NewUniqueIdentifier();
            var response = EncrypterApi.NewEncrypter(_uniqueIdentifier, jointKey, numberOfSelections, byteHash);

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