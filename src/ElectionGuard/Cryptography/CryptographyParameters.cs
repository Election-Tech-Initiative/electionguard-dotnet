using System;

namespace ElectionGuard.SDK.Cryptography
{
    public class CryptographyParameters: IDisposable
    {
        public CryptographyParameters()
        {
            CryptographyApi.NewCryptographyParameters();
        }

        public void Dispose()
        {
            CryptographyApi.FreeCryptographyParameters();
        }
    }
}
