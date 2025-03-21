# GhostsEncoder - Advanced Malware Encryption Tool

![GhostsEncoder Banner](![banner.png](https://github.com/user-attachments/assets/c0faa1a5-c815-437e-9cbd-bb29424887a1)
)

## 🔒 Features

- Multiple encryption algorithms support:
  - AES-256 (CBC/GCM modes)
  - ChaCha20-Poly1305
  - Twofish
  - Serpent
  - Camellia-256
  - Triple DES

- Advanced obfuscation techniques:
  - Polymorphic encryption
  - PE header modification
  - Junk code injection
  - File splitting
  - XOR obfuscation

- Enhanced security features:
  - Strong key derivation (PBKDF2-SHA512)
  - Secure random number generation
  - Multiple encryption layers
  - Signature modification

## 🚀 Usage

### Command Line Mode

```bash
GhostsEncoder -p <password> -i <input_file> [-o <output_file>]
```

Options:
- `-p, --password`: Encryption password
- `-i, --input`: Input file to encrypt
- `-o, --output`: Output file (optional)

Example:
```bash
GhostsEncoder -p MySecretPass123 -i malware.exe -o encrypted.bin
```

## ⚙️ Building from Source

1. Clone the repository:
```bash
git clone https://github.com/ibrahimsql/GhostsEncoder.git
```

2. Navigate to project directory:
```bash
cd GhostsEncoder
```

3. Build the project:
```bash
dotnet build
```

## 📝 Requirements

- .NET 6.0 SDK or later
- Windows/Linux/macOS

## ⚠️ Disclaimer

This tool is created for educational and ethical hacking purposes only. The author is not responsible for any misuse or damage caused by this program. Use it at your own risk.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**ibrahimsql**

* Github: [@ibrahimsql](https://github.com/ibrahimsql)

## 🤝 Contributing

Contributions, issues and feature requests are welcome!

## ❤️ Show your support

Give a ⭐️ if this project helped you! 
