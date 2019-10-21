using System.Runtime.InteropServices;
using ElectionGuard.SDK.Voting.Messages;

namespace ElectionGuard.SDK.Voting.Encrypter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EncryptBallotReturn
    {
        public EncrypterStatus Status;
        public RegisterBallotMessage Message;
        public BallotTracker Tracker;
        public BallotIdentifier Id;
    }
}