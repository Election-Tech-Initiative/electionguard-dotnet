using ElectionGuard.SDK;
using System;
using System.Linq;

namespace UnitTests.Mocks
{
    public static class BallotGenerator
    {
        public static bool[] FillRandomBallot(int numberOfSelections, int expectedNumberOfSelected)
        {
            if (numberOfSelections > Constants.MaxSelections)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSelections));
            }

            // parallel array to use as the look up to set the random index of other array to true
            // if numberOfSelections is 5, this constructs an array of [0, 1, 2, 3, 4]
            var sourceIndexes = Enumerable.Range(0, numberOfSelections).ToArray();

            var random = new Random();
            var selections = new bool[numberOfSelections];
            for (uint i = 0; i < expectedNumberOfSelected; i++)
            {
                var randomSourceIndex = random.Next(sourceIndexes.Length);
                var indexOfSelections = sourceIndexes[randomSourceIndex];
                selections[indexOfSelections] = true;
                // remove the indexOfSelections from the sourceIndexes array
                sourceIndexes = sourceIndexes.Where(val => val != indexOfSelections).ToArray();
            }

            return selections;
        }

        public static bool RandomBit()
        {
            var random = new Random();
            var nextRand = random.Next(2);
            return nextRand == 0;
        }
    }
}