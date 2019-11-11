using System;

namespace ElectionGuard.SDK.Serialization
{
    public static class ByteSerializer
    {
        private const int FromBase = 16;

        public static byte[] SerializeFromRaw(string raw)
        {
            var rawArray = raw.Split('-');
            var array = new byte[rawArray.Length];
            for (var i = 0; i < rawArray.Length; i++) array[i] = Convert.ToByte(rawArray[i], FromBase);
            return array;
        }

        public static byte[] SerializeFromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }
    }
}