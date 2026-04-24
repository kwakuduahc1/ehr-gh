// Ignore Spelling: Neo Auth Sep
using System.Security.Cryptography;

namespace ShimsServer.Controllers.Authentication.Helpers
{
    public static class CodeGenerator
    {
        public static string GetToken() => RandomString(new Random().Next(8, 10));

        static string RandomString(int length)
        {
            string alphabet = "ABCDEFGHIJKLMNPQRTUVWXYZ0123456789";
            var outOfRange = byte.MaxValue + 1 - (byte.MaxValue + 1) % alphabet.Length;

            return string.Concat(
                Enumerable
                    .Repeat(0, byte.MaxValue)
                    .Select(e => RandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte % alphabet.Length])
            );
        }

        static byte RandomByte()
        {
            using var randomizationProvider = RandomNumberGenerator.Create();
            var randomBytes = new byte[1];
            randomizationProvider.GetBytes(randomBytes);
            return randomBytes.Single();
        }
    }
}