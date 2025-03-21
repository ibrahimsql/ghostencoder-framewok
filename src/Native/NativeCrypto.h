#ifndef NATIVE_CRYPTO_H
#define NATIVE_CRYPTO_H

#ifdef __cplusplus
extern "C" {
#endif

#ifdef _WIN32
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT
#endif

// Twofish encryption
EXPORT int TwofishEncrypt(unsigned char* input, int inputLength, unsigned char* output, unsigned char* key, int keyLength);

// Serpent encryption
EXPORT int SerpentEncrypt(unsigned char* input, int inputLength, unsigned char* output, unsigned char* key, int keyLength);

// Camellia encryption
EXPORT int CamelliaEncrypt(unsigned char* input, int inputLength, unsigned char* output, unsigned char* key, int keyLength);

// XTS mode encryption
EXPORT int XtsEncrypt(unsigned char* input, int inputLength, unsigned char* output, unsigned char* key, int keyLength);

#ifdef __cplusplus
}
#endif

#endif // NATIVE_CRYPTO_H 