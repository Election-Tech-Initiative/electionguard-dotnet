using ElectionGuard.SDK.Decryption.Coordinator;
using ElectionGuard.SDK.Decryption.Messages;

namespace ElectionGuard.SDK.Decryption
{
    public class DecryptionTools
    {
        public static bool TrusteeMustBePresent(byte presenceRequested)
        {
            return presenceRequested > 0;
        }
    }
}