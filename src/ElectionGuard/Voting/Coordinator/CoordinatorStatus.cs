namespace ElectionGuard.SDK.Voting.Coordinator
{
    public enum CoordinatorStatus
    {
        Success = 0,
        InsufficientMemory = 1,
        InvalidBallotId = 2,
        InvalidBallot = 3,
        UnregisteredBallot = 4,
        DuplicateBallot = 5,
        TimedOutBallot = 6,
        IoError = 7,
        SerializeError = 8,
        DeserializeError = 9,
    }
}