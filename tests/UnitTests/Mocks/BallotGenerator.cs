using ElectionGuard.SDK;
using System;

namespace UnitTests.Mocks
{
    public static class BallotGenerator
    {
        public static bool[] FillRandomBallot(int numberOfSelections)
        {
            if (numberOfSelections > Constants.MaxSelections)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSelections));
            }

            var selections = new bool[numberOfSelections];
            var selected = false;
            for (uint i = 0; i < numberOfSelections; i++)
            {
                if (!selected)
                {
                    selections[i] = RandomBit();
                }
                else
                {
                    selections[i] = false;
                }
                if (selections[i])
                {
                    selected = true;
                }
            }
            if (!selected)
            {
                selections[numberOfSelections - 1] = true;
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