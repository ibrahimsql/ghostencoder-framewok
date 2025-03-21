using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GhotsEncoder
{
    static class Program
    {
        private static Aes? currentAes;
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;
        private static readonly int Iterations = 100000;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 _____                                                                               _____ 
( ___ )                                                                             ( ___ )
 |   |~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|   | 
 |   |             )     )   (                                                       |   | 
 |   |  (       ( /(  ( /(   )\ )  *   )                                        )    |   | 
 |   |  )\ )    )\()) )\()) (()/(` )  /(   (               (    (            ( /(    |   | 
 |   | (()/(   ((_)\ ((_)\   /(_))( )(_))  )\    (      (  )(   )\ )  `  )   )\())   |   | 
 |   |  /(_))_  _((_)  ((_) (_)) (_(_())  ((_)   )\ )   )\(()\ (()/(  /(/(  (_))/    |   | 
 |   | (_)) __|| || | / _ \ / __||_   _|  | __| _(_/(  ((_)((_) )(_))((_)_\ | |_     |   | 
 |   |   | (_ || __ || (_) |\__ \  | |    | _| | ' \))/ _|| '_|| || || '_ \)|  _|    |   | 
 |   |    \___||_||_| \___/ |___/  |_|    |___||_||_| \__||_|   \_, || .__/  \__|    |   | 
 |   |                                                          |__/ |_|             |   | 
 |___|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|___| 
(_____)                                                                             (_____) ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Digital Asset Security Framework v1.5");
            Console.WriteLine("Developer by: ibrahimsql");
            Console.ResetColor();
            string? password = null;
            string? inputFile = null;
            string? outputFile = null;
            string? algorithm = "aes"; // Default algorithm

            if (args.Length < 4)
            {
                ShowUsage();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-p":
                    case "--password":
                        if (i + 1 < args.Length) password = args[++i];
                        break;
                    case "-i":
                    case "--input":
                        if (i + 1 < args.Length) inputFile = args[++i];
                        break;
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length) outputFile = args[++i];
                        break;
                    case "-a":
                    case "--algorithm":
                        if (i + 1 < args.Length) algorithm = args[++i].ToLower();
                        break;
                }
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine("Error: Password and input file are required.");
                ShowUsage();
                return;
            }

            if (!IsValidAlgorithm(algorithm))
            {
                Console.WriteLine($"Error: Invalid algorithm '{algorithm}'");
                ShowUsage();
                return;
            }

            if (string.IsNullOrEmpty(outputFile))
            {
                string dir = Path.GetDirectoryName(inputFile) ?? ".";
                outputFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(inputFile) + ".encrypted");
            }

            try
            {
                Console.WriteLine($"Starting enhanced malware encryption using {algorithm.ToUpper()}...");
                
                switch (algorithm)
                {
                    case "aes":
                        AdvancedEncryption.EncryptMalware(inputFile, outputFile, password, progress =>
                        {
                            Console.Write($"\rProgress: {progress}%");
                            if (progress == 100) Console.WriteLine();
                        });
                        break;
                        
                    case "twofish":
                        AdvancedEncryption.EncryptFileWithTwofish(password, inputFile, outputFile, progress =>
                        {
                            Console.Write($"\rProgress: {progress}%");
                            if (progress == 100) Console.WriteLine();
                        });
                        break;
                        
                    case "chacha20":
                        var fileContent = File.ReadAllText(inputFile);
                        var (cipherText, nonce, tag) = AdvancedEncryption.ChaCha20Poly1305Encrypt(fileContent, password);
                        File.WriteAllText(outputFile, cipherText);
                        break;
                        
                    case "serpent":
                        var content = File.ReadAllText(inputFile);
                        var encrypted = AdvancedEncryption.SerpentEncrypt(content, password);
                        File.WriteAllText(outputFile, encrypted);
                        break;
                        
                    case "camellia":
                        var text = File.ReadAllText(inputFile);
                        var encryptedText = AdvancedEncryption.CamelliaEncrypt(text, password);
                        File.WriteAllText(outputFile, encryptedText);
                        break;
                }
                
                Console.WriteLine($"\nFile successfully encrypted: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static bool IsValidAlgorithm(string algorithm)
        {
            var validAlgorithms = new[] { "aes", "twofish", "chacha20", "serpent", "camellia" };
            return Array.Exists(validAlgorithms, a => a == algorithm.ToLower());
        }

        static void ShowUsage()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("  GhostsEncoder -p <password> -i <input_file> [-o <output_file>] [-a <algorithm>]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  -p, --password    Password for encryption");
            Console.WriteLine("  -i, --input       Input file to encrypt");
            Console.WriteLine("  -o, --output      Output file (optional, default: input_file.encrypted)");
            Console.WriteLine("  -a, --algorithm   Encryption algorithm (optional, default: aes)");
            Console.WriteLine("\nAvailable Algorithms:");
            Console.WriteLine("  - aes      (AES-256 CBC)");
            Console.WriteLine("  - twofish  (Twofish-256)");
            Console.WriteLine("  - chacha20 (ChaCha20-Poly1305)");
            Console.WriteLine("  - serpent  (Serpent-256)");
            Console.WriteLine("  - camellia (Camellia-256)");
            Console.WriteLine("\nExamples:");
            Console.WriteLine("  GhostsEncoder -p MySecretPass123 -i malware.exe -o encrypted.bin -a aes");
            Console.WriteLine("  GhostsEncoder -p StrongPassword! -i secret.txt -a twofish");
            Console.WriteLine("  GhostsEncoder -p Secure123! -i data.bin -a chacha20");
        }

        #region Encryption Methods
        private static void EncryptFileWithAesCbc(string password, string inputFile, string outputFile)
        {
            using (currentAes = Aes.Create())
            {
                currentAes.KeySize = KeySize;
                currentAes.BlockSize = BlockSize;
                currentAes.Mode = CipherMode.CBC;
                currentAes.Padding = PaddingMode.PKCS7;

                // Generate salt and derive key
                byte[] salt = GenerateRandomBytes(32);
                byte[] keyBytes = CreateKey(password, salt, KeySize, Iterations);
                byte[] iv = GenerateRandomBytes(16);

                // Create output file
                using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    // Write salt and IV to beginning of the file
                    outputStream.Write(salt, 0, salt.Length);
                    outputStream.Write(iv, 0, iv.Length);

                    // Create cryptographic stream
                    using (var encryptor = currentAes.CreateEncryptor())
                    using (var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    {
                        // Read and encrypt file in chunks
                        using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            long totalBytes = inputStream.Length;
                            long bytesProcessed = 0;

                            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cryptoStream.Write(buffer, 0, bytesRead);
                                bytesProcessed += bytesRead;
                                
                                // Report progress
                                int progressPercentage = (int)(bytesProcessed * 100 / totalBytes);
                                ShowProgress(progressPercentage);
                            }
                        }
                    }
                }
            }
            
            // Add file info as a comment
            FileInfo fileInfo = new FileInfo(outputFile);
            long fileSize = fileInfo.Length;
            string fileSizeStr = GetReadableFileSize(fileSize);
            
            Console.WriteLine();
            Console.WriteLine("╔══ Ibrahim's Encryption Report ═══╗");
            Console.WriteLine($"File size: {fileSizeStr}");
            Console.WriteLine($"Encryption algorithm: {currentAes.Mode} mode with {currentAes.KeySize}-bit key");
            Console.WriteLine($"Password iterations: {Iterations}");
            Console.WriteLine($"Creation time: {DateTime.Now}");
            Console.WriteLine("╚═════════════════════════════════╝");
        }
        
        private static string GetReadableFileSize(long fileSizeInBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = fileSizeInBytes;
            
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            
            return $"{size:0.##} {sizes[order]}";
        }

        private static byte[] CreateKey(string password, byte[] salt, int keySize, int iterations)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                return deriveBytes.GetBytes(keySize / 8);
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

        // Enhanced progress display
        private static int lastProgress = -1;
        private static void ShowProgress(int progress)
        {
            if (progress == lastProgress)
                return;
                
            lastProgress = progress;
            
            const int barWidth = 50;
            int fillWidth = (int)((progress * barWidth) / 100.0);
            
            Console.Write("\r[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', fillWidth));
            Console.ResetColor();
            Console.Write(new string(' ', barWidth - fillWidth));
            Console.Write($"] {progress}% - Ibrahim's Encryption Tool");
            
            if (progress >= 100)
                Console.WriteLine();
        }
        #endregion
    }
}
