#pragma once
#include <cstdint>
#include <istream>
#include <ostream>
#include <vector>

#include <cppcodec/base64_rfc4648.hpp>
#include <cppcodec/hex_lower.hpp>

using base64 = cppcodec::base64_rfc4648;
using hex = cppcodec::hex_lower;

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
std::string EncodeBase64(const uint8_t* data, size_t size) {
    return base64::encode(data, size);
}

// **Base64 解码**
std::vector<uint8_t> DecodeBase64(const std::string& encoded) {
    return base64::decode(encoded.data(), encoded.size());
}

// **Hex 编码**
std::string EncodeHex(const uint8_t* data, size_t size) {
    return hex::encode(data, size);
}

// **Hex 解码**
std::vector<uint8_t> DecodeHex(const std::string& encoded) {
    return base64::decode(encoded.data(), encoded.size());
}
