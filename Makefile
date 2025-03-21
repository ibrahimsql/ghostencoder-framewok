CC = gcc
CFLAGS = -Wall -O2 -fPIC
LDFLAGS = -shared

# Windows specific
ifeq ($(OS),Windows_NT)
	TARGET = NativeCrypto.dll
	LDFLAGS = -shared -Wl,--out-implib,libNativeCrypto.a
else
	# Linux specific
	TARGET = libNativeCrypto.so
endif

all: $(TARGET)

$(TARGET): NativeCrypto.c
	$(CC) $(CFLAGS) -o $@ $^ $(LDFLAGS)

clean:
	rm -f $(TARGET) libNativeCrypto.a

.PHONY: all clean 