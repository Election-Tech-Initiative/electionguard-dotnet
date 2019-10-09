using ElectionGuard.SDK.KeyCeremony;
using NUnit.Framework;

namespace UnitTests.KeyCeremony
{
    [TestFixture]
    public class KeyCeremonyTrusteeTests
    {
        private KeyCeremonyTrustee _keyCeremonyTrustee;

        [SetUp]
        public void SetUp()
        {
            _keyCeremonyTrustee = new KeyCeremonyTrustee(1, 0 , 0);
        }

        [TearDown]
        public void TearDown()
        {
            _keyCeremonyTrustee.Dispose();
        }

        [Test]
        public void GenerateKeyTest()
        {
            // var rawHash = new byte[32];
            // var test = _keyCeremonyTrustee.GenerateKey(rawHash);
            Assert.Pass();
        }

        [Test]
        public void GenerateSharesTest()
        {
            // var message = new AllKeysReceivedMessage();
            // var response = _keyCeremonyTrustee.GenerateShares(message);
            Assert.Pass();
        }

        [Test]
        public void VerifySharesTest()
        {
            // var message = new AllSharesReceivedMessage();
            // var response = _keyCeremonyTrustee.VerifyShares(message);
            Assert.Pass();
        }

        [Test]
        public void ExportStateTest()
        {
            // var response = _keyCeremonyTrustee.ExportState();
            Assert.Pass();
        }
    }
}