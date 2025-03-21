using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GhotsEncoder
{
    /// <summary>
    /// Native crypto operations implementation
    /// </summary>
    public static class NativeCrypto
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Applies polymorphic encryption to the input data
        /// </summary>
        public static void PolymorphicEncrypt(
            byte[] input, int inputLength,
            byte[] key, int keyLength,
            byte[] iv, int ivLength,
            byte[] output, int outputLength,
            byte[] signature, int signatureLength)
        {
            // Generate random signature
            rng.GetBytes(signature);

            // Apply polymorphic encryption (simulated with AES)
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    // Add some randomization to make each encryption unique
                    byte[] salt = new byte[16];
                    rng.GetBytes(salt);
                    
                    // Combine salt with input
                    byte[] combined = new byte[salt.Length + input.Length];
                    Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                    Buffer.BlockCopy(input, 0, combined, salt.Length, input.Length);
                    
                    // Encrypt
                    byte[] encrypted = encryptor.TransformFinalBlock(combined, 0, combined.Length);
                    Buffer.BlockCopy(encrypted, 0, output, 0, encrypted.Length);
                }
            }
        }

        /// <summary>
        /// Obfuscates executable by adding junk code
        /// </summary>
        public static void ObfuscateExecutable(byte[] data, int length, string method)
        {
            switch (method.ToLower())
            {
                case "junk":
                    AddJunkCode(data, length);
                    break;
                case "shuffle":
                    ShuffleCode(data, length);
                    break;
                default:
                    AddJunkCode(data, length);
                    break;
            }
        }

        private static void AddJunkCode(byte[] data, int length)
        {
            // Add random junk bytes at specific intervals
            for (int i = 0; i < length; i += 1024)
            {
                if (i + 16 <= length)
                {
                    rng.GetBytes(data, i, 16);
                }
            }
        }

        private static void ShuffleCode(byte[] data, int length)
        {
            // Shuffle non-critical sections of the code
            var rnd = new Random();
            for (int i = 0; i < length; i += 512)
            {
                if (i + 32 <= length)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        int idx1 = i + rnd.Next(32);
                        int idx2 = i + rnd.Next(32);
                        byte temp = data[idx1];
                        data[idx1] = data[idx2];
                        data[idx2] = temp;
                    }
                }
            }
        }

        private const string LibraryName = "NativeCrypto";

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TwofishEncrypt(byte[] input, int inputLength, byte[] output, byte[] key, int keyLength);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SerpentEncrypt(byte[] input, int inputLength, byte[] output, byte[] key, int keyLength);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CamelliaEncrypt(byte[] input, int inputLength, byte[] output, byte[] key, int keyLength);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XtsEncrypt(byte[] input, int inputLength, byte[] output, byte[] key, int keyLength);
    }
} 