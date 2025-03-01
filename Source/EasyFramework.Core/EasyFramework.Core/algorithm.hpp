#pragma once
#include <cstdint>
#include <istream>
#include <ostream>
#include <vector>

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
