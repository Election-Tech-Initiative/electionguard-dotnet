using System;
using System.Collections.Generic;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;

namespace ElectionGuard.SDK
{
    public class Election
    {
        private byte[] _bashHash;
        private readonly byte[] _testBaseHash = { 0, 0xff, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Election (int numberOfTrustees, int threshold, ContestFormat format)
        {
            NumberOfTrustees = numberOfTrustees;
            Threshold = threshold;
            PublicJointKey = null;
            TrusteeKeys = new Dictionary<int, string>();
            NumberOfSelections = 0;

            GenerateBaseHash();
            KeyCeremony();
            CalculateSelections(format);
        }

        public int NumberOfTrustees { get; }

        public int Threshold { get; }

        public string PublicJointKey { get; private set; }

        public Dictionary<int, string> TrusteeKeys { get; private set; }

        public int NumberOfSelections { get; private set; }

        public string BaseHashCode => Convert.ToBase64String(_testBaseHash);

        private void GenerateBaseHash()
        {
            // TODO Properly Generate Base Hash
            _bashHash = _testBaseHash;
        }

        private void KeyCeremony()
        {
            var keyCeremonyProcessor = new KeyCeremonyProcessor(NumberOfTrustees, Threshold, _bashHash);
            PublicJointKey = keyCeremonyProcessor.GeneratePublicJointKey();
            TrusteeKeys = keyCeremonyProcessor.GenerateTrusteeKeys();
        }

        private void CalculateSelections(ContestFormat format)
        {
            // TODO Properly Calculate Selections
            const int numberOfSelections = 10;
            if (numberOfSelections > MaxValues.MaxSelections)
            {
                throw new Exception("Max Selections Exceeded.");
            }
            NumberOfSelections = numberOfSelections;
        }
    }
}
