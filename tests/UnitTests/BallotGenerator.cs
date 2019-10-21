using System;
using ElectionGuard.SDK.Config;

namespace UnitTests
{
    public static class BallotGenerator
    {
        public static bool[] FillRandomBallot(uint numberOfSelections)
        {
            if (numberOfSelections > MaxValues.MaxSelections)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfSelections));
            }

            var selections = new bool[numberOfSelections];
            var selected = false;
            for (uint i = 0; i < numberOfSelections; i++)
            {
                if (!selected)
                {
                    selections[i] = true;
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
            return selections;
        }

        public static bool RandomBit()
        {
            var random = new Random();
            return random.Next() > (int.MaxValue / 2);
        }
    }
}