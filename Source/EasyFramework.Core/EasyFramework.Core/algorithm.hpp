#pragma once
#include <cstdint>
#include <istream>
#include <ostream>
#include <vector>

#include <cryptopp/cryptlib.h>
#include <cryptopp/base64.h>
#include <cryptopp/hex.h>
#include <cryptopp/filters.h>

using namespace CryptoPP;

inline void WriteVarint32(uint32_t value, std::ostream& stream) {
    while (value >= 0x80) {
        stream << static_cast<uint8_t>((value & 0x7F) | 0x80);
        value >>= 7;
    }
    stream << static_cast<uint8_t>(value);
}

inline uint32_t ReadVarint32(std::istream& stream) {
    uint32_t result = 0;
    int shift = 0;
    while (true) {
        uint8_t byte;
        stream >> byte;

        result |= (byte & 0x7F) << shift;
        if ((byte & 0x80) == 0) break;
        shift += 7;
    }
    return result;
}

// **Base64 编码**
std::string EncodeBase64(const byte* data, size_t size) {
    std::string encoded;
    StringSource(data, size, true,
        new Base64Encoder(new StringSink(encoded), false)); // false 禁用换行
    return encoded;
}

// **Base64 解码**
std::vector<uint8_t> DecodeBase64(const std::string& encoded) {
    std::vector<uint8_t> decoded;
    StringSource(encoded, true,
        new Base64Decoder(new VectorSink(decoded)));
    return decoded;
}

// **Hex 编码**
std::string EncodeHex(const byte* data, size_t size) {
    std::string encoded;
    StringSource(data, size, true,
        new HexEncoder(new StringSink(encoded), false)); // false 禁用换行
    return encoded;
}

// **Hex 解码**
std::vector<uint8_t> DecodeHex(const std::string& encoded) {
    std::vector<uint8_t> decoded;
    StringSource(encoded, true,
        new HexDecoder(new VectorSink(decoded)));
    return decoded;
}
