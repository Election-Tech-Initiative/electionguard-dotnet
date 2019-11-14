using System;
using System.IO;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.KeyCeremony;

namespace ElectionGuard.SDK.Serialization
{
    public static class JointKeySerializer
    {
        public static byte[] Serialize(JointPublicKeyResponse response)
        {
            return ByteSerializer.SerializeFromBase64(response.Base64);
        }

        public static JointPublicKeyResponse Deserialize(byte[] raw)
        {
            using var stream = new MemoryStream(raw);
            using var reader = new BinaryReader(stream);

            var response = new JointPublicKeyResponse
            {
                Length = raw.Length,
                Raw = BitConverter.ToString(raw),
                Base64 = Convert.ToBase64String(raw),
                NumberOfTrustees = reader.ReadUInt32(),
                PublicKey = new ulong[Constants.Uint4096WordCount]
            };
            for (var i = 0; i < Constants.Uint4096WordCount; i++) response.PublicKey[i] = reader.ReadUInt64();
            if (reader.PeekChar() != Constants.EndOfFile) throw new EndOfStreamException();
            return response;
        }
    }
}