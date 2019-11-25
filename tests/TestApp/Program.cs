using ElectionGuard.SDK;
using ElectionGuard.SDK.Models;

/// <summary>
/// Simple console app to use as a playground for running the C# SDK methods
/// This is easier to run when we want to use gdb to debug into the C methods
/// that get called by the C# side.
/// </summary>
namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ElectionGuardConfig()
            {
                NumberOfTrustees = 3,
                Threshold = 3,
                SubgroupOrder = 0,
                ElectionMetadata = "placeholder",
            };
            var manifest = new ElectionManifest()
            {
                Contests = new Contest[]{ new YesNoContest()
                {
                    Type = "YesNo"
                } },
            };

            var result = Election.CreateElection(config, manifest);

        }
    }
}
