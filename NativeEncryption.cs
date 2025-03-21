using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace GhotsEncoder
{
    /// <summary>
    /// Provides access to native encryption algorithms implemented in C/C++
    /// This class uses P/Invoke to call into a native library for maximum performance
    /// </summary>
    public static class NativeEncryption
    {
        // Path to the native library
        private const string NativeLibrary = "NativeCrypto.dll";

        // Native methods for encryption
        [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        private static extern int EncryptDataXTS(
            [In] byte[] plaintext, 
            int plaintextLength,
            [In] byte[] key, 
            int keyLength,
            [In] byte[] tweak,
            [Out] byte[] ciphertext,
            int ciphertextLength);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        private static extern int HashData(
            [In] byte[] data,
            int dataLength,
            [In] string algorithm,
            [Out] byte[] hashValue,
            int hashValueLength);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention.Cdecl)]
        private static extern int PolymorphicEncrypt(
            [In] byte[] plainData,
            int plainDataLength,
            [In] byte[] key,
            int keyLength,
            [In] byte[] nonce,
            int nonceLength,
            [Out] byte[] cipherData,
            int cipherDataLength,
            [Out] byte[] signature,
            int signatureLength);

        // Wrapper methods with fallback implementations
        
        /// <summary>
        /// Encrypts data using XTS mode encryption (optimized for disk encryption)
        /// Falls back to AES-CBC if native library is not available
        /// </summary>
        public static byte[] EncryptWithXTS(byte[] data, string password)
        {
            try
            {
                // Generate key material
                byte[] salt = GenerateRandomBytes(32);
                byte[] keyMaterial = CreateKey(password, salt, 512); // Need 512 bits for XTS
                
                // Split into two keys (XTS uses two keys)
                byte[] key1 = new byte[32];
                byte[] key2 = new byte[32];
                Buffer.BlockCopy(keyMaterial, 0, key1, 0, 32);
                Buffer.BlockCopy(keyMaterial, 32, key2, 0, 32);
                
                // Combine keys for native call
                byte[] combinedKey = new byte[64];
                Buffer.BlockCopy(key1, 0, combinedKey, 0, 32);
                Buffer.BlockCopy(key2, 0, combinedKey, 32, 32);
                
                // Generate tweak (usually sector number in disk encryption)
                byte[] tweak = GenerateRandomBytes(16);
                
                // Output buffer (same size as input for XTS)
                byte[] ciphertext = new byte[data.Length];
                
                // Call native method
                int result = EncryptDataXTS(data, data.Length, combinedKey, combinedKey.Length, 
                    tweak, ciphertext, ciphertext.Length);
                
                if (result >= 0)
                {
                    // Construct result with salt and tweak
                    byte[] output = new byte[1 + salt.Length + tweak.Length + ciphertext.Length];
                    output[0] = (byte)salt.Length;
                    Buffer.BlockCopy(salt, 0, output, 1, salt.Length);
                    Buffer.BlockCopy(tweak, 0, output, 1 + salt.Length, tweak.Length);
                    Buffer.BlockCopy(ciphertext, 0, output, 1 + salt.Length + tweak.Length, ciphertext.Length);
                    
                    return output;
                }
                else
                {
                    // Fall back to AES-CBC if native call failed
                    Console.WriteLine("Native XTS encryption failed, falling back to AES-CBC");
                    return FallbackEncrypt(data, password);
                }
            }
            catch (DllNotFoundException)
            {
                // Native library not found, use fallback
                Console.WriteLine("Native library not found, falling back to AES-CBC");
                return FallbackEncrypt(data, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in XTS encryption: {ex.Message}, falling back to AES-CBC");
                return FallbackEncrypt(data, password);
            }
        }
        
        /// <summary>
        /// Encrypts a file using polymorphic encryption that changes its signature with each encryption
        /// </summary>
        public static void PolymorphicEncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(inputFile);
                byte[] salt = GenerateRandomBytes(32);
                byte[] key = CreateKey(password, salt, 256);
                byte[] nonce = GenerateRandomBytes(16);
                byte[] signature = new byte[64]; // For HMAC or signature
                
                // Output buffer (may be larger than input due to padding)
                byte[] cipherData = new byte[fileData.Length + 64]; // Extra space for padding
                
                try
                {
                    // Try native implementation
                    int result = PolymorphicEncrypt(fileData, fileData.Length, key, key.Length,
                        nonce, nonce.Length, cipherData, cipherData.Length, signature, signature.Length);
                    
                    if (result >= 0)
                    {
                        // Create output format
                        using (var output = new FileStream(outputFile, FileMode.Create))
                        {
                            // Write header with version
                            output.WriteByte(0x01); // Version
                            
                            // Write salt and nonce
                            output.WriteByte((byte)salt.Length);
                            output.Write(salt, 0, salt.Length);
                            output.WriteByte((byte)nonce.Length);
                            output.Write(nonce, 0, nonce.Length);
                            
                            // Write signature
                            output.WriteByte((byte)signature.Length);
                            output.Write(signature, 0, signature.Length);
                            
                            // Write encrypted data length and data
                            byte[] lengthBytes = BitConverter.GetBytes(result);
                            output.Write(lengthBytes, 0, 4);
                            output.Write(cipherData, 0, result);
                        }
                        
                        Console.WriteLine("Polymorphic encryption completed using native implementation");
                        return;
                    }
                    
                    // Fall back if native call failed
                    Console.WriteLine("Native polymorphic encryption failed, using fallback");
                }
                catch (DllNotFoundException)
                {
                    Console.WriteLine("Native library not found, using fallback encryption");
                }
                
                // Fallback implementation
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = nonce;
                    
                    // Create output file
                    using (var output = new FileStream(outputFile, FileMode.Create))
                    {
                        // Write header
                        output.WriteByte(0x02); // Fallback version
                        
                        // Write salt and IV
                        output.WriteByte((byte)salt.Length);
                        output.Write(salt, 0, salt.Length);
                        output.WriteByte((byte)nonce.Length);
                        output.Write(nonce, 0, nonce.Length);
                        
                        // Encrypt the data
                        using (var encryptor = aes.CreateEncryptor())
                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(fileData, 0, fileData.Length);
                            cs.FlushFinalBlock();
                            
                            byte[] encrypted = ms.ToArray();
                            
                            // Compute HMAC
                            using (var hmac = new HMACSHA256(key))
                            {
                                byte[] hmacValue = hmac.ComputeHash(encrypted);
                                
                                // Write HMAC
                                output.WriteByte((byte)hmacValue.Length);
                                output.Write(hmacValue, 0, hmacValue.Length);
                                
                                // Write encrypted data
                                byte[] lengthBytes = BitConverter.GetBytes(encrypted.Length);
                                output.Write(lengthBytes, 0, 4);
                                output.Write(encrypted, 0, encrypted.Length);
                            }
                        }
                    }
                    
                    Console.WriteLine("Polymorphic encryption completed using fallback implementation");
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Error in polymorphic encryption: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Computes a hash using a variety of algorithms (uses native implementation if available)
        /// </summary>
        public static byte[] ComputeHash(byte[] data, string algorithm)
        {
            // Determine hash size
            int hashSize;
            switch (algorithm.ToUpper())
            {
                case "MD5": hashSize = 16; break;
                case "SHA1": hashSize = 20; break;
                case "SHA256": hashSize = 32; break;
                case "SHA384": hashSize = 48; break;
                case "SHA512": hashSize = 64; break;
                case "BLAKE2B": hashSize = 64; break;
                default: throw new ArgumentException($"Unsupported hash algorithm: {algorithm}");
            }
            
            byte[] hashValue = new byte[hashSize];
            
            try
            {
                // Try native implementation first
                int result = HashData(data, data.Length, algorithm, hashValue, hashValue.Length);
                
                if (result >= 0)
                {
                    return hashValue;
                }
                
                // Fall back to .NET implementation
                Console.WriteLine($"Native {algorithm} hashing failed, using fallback");
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine($"Native library not found, using fallback for {algorithm}");
            }
            
            // Fallback implementations
            using (HashAlgorithm hash = GetHashAlgorithm(algorithm))
            {
                return hash.ComputeHash(data);
            }
        }
        
        // Helper method to get appropriate hash algorithm
        private static HashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm.ToUpper())
            {
                case "MD5": return MD5.Create();
                case "SHA1": return SHA1.Create();
                case "SHA256": return SHA256.Create();
                case "SHA384": return SHA384.Create();
                case "SHA512": return SHA512.Create();
                case "BLAKE2B": 
                    throw new NotSupportedException("BLAKE2B is only supported via native library");
                default:
                    throw new ArgumentException($"Unsupported hash algorithm: {algorithm}");
            }
        }
        
        // Fallback encryption method (AES-CBC)
        private static byte[] FallbackEncrypt(byte[] data, string password)
        {
            byte[] salt = GenerateRandomBytes(32);
            byte[] key = CreateKey(password, salt, 256);
            byte[] iv = GenerateRandomBytes(16);
            
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;
                
                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    // Write salt and IV
                    ms.WriteByte((byte)salt.Length);
                    ms.Write(salt, 0, salt.Length);
                    ms.Write(iv, 0, iv.Length);
                    
                    // Encrypt data
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    
                    return ms.ToArray();
                }
            }
        }
        
        // Helper methods
        private static byte[] CreateKey(string password, byte[] salt, int keySizeBits)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512))
            {
                return deriveBytes.GetBytes(keySizeBits / 8);
            }
        }
        
        private static byte[] GenerateRandomBytes(int size)
        {
            var randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
} 