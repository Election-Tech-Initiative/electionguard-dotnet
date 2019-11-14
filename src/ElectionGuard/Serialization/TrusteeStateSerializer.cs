using System;
using System.IO;
using ElectionGuard.SDK.KeyCeremony;
using ElectionGuard.SDK.StateManagement;

namespace ElectionGuard.SDK.Serialization
{
    public static class TrusteeStateSerializer
    {
        public static byte[] Serialize(TrusteeStateExport response)
        {
            return ByteSerializer.SerializeFromBase64(response.Base64);
        }

        public static TrusteeStateExport Deserialize(byte[] raw, int numberOfTrustees)
        {
            using var stream = new MemoryStream(raw);
            using var reader = new BinaryReader(stream);

            var response = new TrusteeStateExport
            {
                Length = raw.Length,
                Raw = BitConverter.ToString(raw),
                Base64 = Convert.ToBase64String(raw),
                Index = reader.ReadUInt32(),
                Threshold = reader.ReadUInt32()
            };
            response.PrivateKeyCoefficients = new ulong[response.Threshold][];
            for (var i = 0; i < response.Threshold; i++)
            {
                response.PrivateKeyCoefficients[i] = new ulong[64];
                for (var j = 0; j < 64; j++) response.PrivateKeyCoefficients[i][j] = reader.ReadUInt64();
            }

            response.RsaE = ReadUInt64Array(reader, 1);
            response.RsaQ = ReadUInt64Array(reader, 32);
            response.RsaD = ReadUInt64Array(reader, 64);
            response.RsaP = ReadUInt64Array(reader, 32);
            response.RsaN = ReadUInt64Array(reader, 64);

            response.EncryptedKeyShares = new ulong[numberOfTrustees][];
            for (var i = 0; i < numberOfTrustees; i++) response.EncryptedKeyShares[i] = ReadUInt64Array(reader, 12);

            if (reader.PeekChar() != Constants.EndOfFile) throw new EndOfStreamException();
            return response;
        }

        private static ulong[] ReadUInt64Array(BinaryReader reader, int count)
        {
            var temp = new ulong[count];
            for (var i = 0; i < count; i++) temp[i] = reader.ReadUInt64();
            return temp;
        }
    }
}