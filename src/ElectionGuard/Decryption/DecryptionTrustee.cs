using System;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.Decryption.Trustee;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.StateManagement;
using ElectionGuard.SDK.Utility;

namespace ElectionGuard.SDK.Decryption
{
    public class DecryptionTrustee : SafePointer, IDisposable
    {
        private UIntPtr _trustee;

        public DecryptionTrustee(uint numberOfTrustees, uint threshold, uint numberOfSelections,
            TrusteeStateExport trusteeStateExport, byte[] baseHash)
        {
            var trusteeState = TrusteeApi.NewTrusteeState(trusteeStateExport);
            Initialize(numberOfTrustees, threshold, numberOfSelections, trusteeState, baseHash);
            TrusteeApi.FreeTrusteeState(trusteeState);
        }


        public DecryptionTrustee(uint numberOfTrustees, uint threshold, uint numberOfSelections, TrusteeState trusteeState, byte[] baseHash)
        {
            Initialize(numberOfTrustees, threshold, numberOfSelections, trusteeState, baseHash);
        }

        private void Initialize(uint numberOfTrustees, uint threshold, uint numberOfSelections, TrusteeState trusteeState, byte[] baseHash)
        {
            var response = TrusteeApi.NewTrustee(numberOfTrustees, threshold, numberOfSelections, trusteeState, baseHash);
            if (response.Status == TrusteeStatus.Success)
            {
                _trustee = response.Trustee;
            }
            else
            {
                throw new Exception("Failed to create decryption trustee");
            }
        }

        public TrusteeStatus TallyVotingRecord(File file)
        {
            return Protect(_trustee, () => TrusteeApi.TallyVotingRecord(_trustee, file));
        }

        public ComputeShareReturn ComputeShare()
        {
            return Protect(_trustee, () => TrusteeApi.ComputeShare(_trustee));
        }

        public ComputeFragmentsReturn ComputeFragments(DecryptionFragmentsRequest fragmentsRequest)
        {
            return Protect(_trustee, () => TrusteeApi.ComputeFragments(_trustee, fragmentsRequest));
        }


        public void Dispose()
        {
            ProtectVoid(_trustee, () => TrusteeApi.FreeTrustee(_trustee));
        }
    }
}