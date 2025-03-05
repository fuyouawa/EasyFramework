#include "serializer.h"
#include "stream_wrapper.hpp"
#include "archive_wrapper.hpp"

OutputArchive AllocBinaryOutputArchive(IoStream stream) {
    auto archive = new BinaryOutputArchiveWrapper(*GetStream(stream)->stream());
    auto ret = OutputArchive();
    ret.ptr = archive;
    return ret;
}

void FreeOutputArchive(OutputArchive archive) {
    delete GetArchive(archive);
}

InputArchive AllocBinaryInputArchive(IoStream stream) {
    auto archive = new BinaryInputArchiveWrapper(*GetStream(stream)->stream());
    auto ret = InputArchive();
    ret.ptr = archive;
    return ret;
}

OutputArchive AllocJsonOutputArchive(IoStream stream) {
    auto archive = new JsonOutputArchiveWrapper(*GetStream(stream)->stream());
    auto ret = OutputArchive();
    ret.ptr = archive;
    return ret;
}

InputArchive AllocJsonInputArchive(IoStream stream) {
    auto archive = new JsonInputArchiveWrapper(*GetStream(stream)->stream());
    auto ret = InputArchive();
    ret.ptr = archive;
    return ret;
}

void FreeInputArchive(InputArchive archive) {
    delete GetArchive(archive);
}

void OutputArchiveSetNextName(OutputArchive archive, const char* name) {
    auto str = std::string();
    if (name != nullptr) {
        str = name;
    }
    GetArchive(archive)->set_next_name(std::move(str));
}

void InputArchiveSetNextName(InputArchive archive, const char* name) {
    auto str = std::string();
    if (name != nullptr) {
        str = name;
    }
    GetArchive(archive)->set_next_name(std::move(str));
}

void OutputArchiveStartNode(OutputArchive archive) {
    GetArchive(archive)->StartNode();
}

void OutputArchiveFinishNode(OutputArchive archive) {
    GetArchive(archive)->FinishNode();
}

void InputArchiveStartNode(InputArchive archive) {
    GetArchive(archive)->StartNode();
}

void InputArchiveFinishNode(InputArchive archive) {
    GetArchive(archive)->FinishNode();
}

void WriteSizeToOutputArchive(OutputArchive archive, uint32_t size) {
    auto arch = GetArchive(archive);
    if (arch->type() == ArchiveType::Binary) {
        auto v = Varint32(size);
        arch->Process(v);
    }
    else {
        auto s = static_cast<uint64_t>(size);
        arch->Process(cereal::make_size_tag(s));
    }
}

uint32_t ReadSizeFromInputArchive(InputArchive archive) {
    auto arch = GetArchive(archive);
    if (arch->type() == ArchiveType::Binary) {
        auto v = Varint32();
        arch->Process(v);
        return v.value;
    }
    else {
        auto size = uint64_t();
        arch->Process(cereal::make_size_tag(size));
        return static_cast<uint32_t>(size);
    }
}

#define NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(type_name, type) \
void Write##type_name##ToOutputArchive(OutputArchive archive, type value) { \
    GetArchive(archive)->Process(value); \
} \
type Read##type_name##FromInputArchive(InputArchive archive) { \
    auto ret = type(); \
    GetArchive(archive)->Process(ret); \
    return ret; \
}

NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Int64, int64_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Int32, int32_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Int16, int16_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Int8, int8_t)

NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(UInt64, uint64_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(UInt32, uint32_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(UInt16, uint16_t)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(UInt8, uint8_t)

void WriteBoolToOutputArchive(OutputArchive archive, uint8_t value) {
    auto b = value != 0;
    GetArchive(archive)->Process(b);
}

uint8_t ReadBoolFromInputArchive(InputArchive archive) {
    auto b = bool();
    GetArchive(archive)->Process(b);
    return b ? 1 : 0;
}

NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Float, float)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Double, double)

void WriteVarint32ToOutputArchive(OutputArchive archive, uint32_t value) {
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        arch->Process(value);
    }
    else {
        auto v = Varint32(value);
        arch->Process(v);
    }
}

uint32_t ReadVarint32FromInputArchive(InputArchive archive) {
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        uint32_t ret;
        arch->Process(ret);
        return ret;
    }
    else {
        auto ret = Varint32();
        arch->Process(ret);
        return ret.value;
    }
}

void WriteBinaryToOutputArchive(OutputArchive archive, Buffer buffer) {
    auto arch = GetArchive(archive);
    if (arch->type() == ArchiveType::Binary) {
        auto v = Varint32(buffer.size);
        arch->Process(v);
    }
    auto data = std::vector<uint8_t>(buffer.ptr, buffer.ptr + buffer.size);
    arch->Process(data);
}

Buffer ReadBinaryFromInputArchive(InputArchive archive) {
    auto size = size_t();
    auto arch = GetArchive(archive);
    if (arch->type() == ArchiveType::Binary) {
        auto v = Varint32();
        arch->Process(v);
        size = v.value;
    }
    auto buf = AllocBuffer(static_cast<uint32_t>(size));
    auto data = std::vector<uint8_t>(size);
    GetArchive(archive)->Process(data);
    memcpy_s(buf.ptr, buf.size, data.data(), data.size());
    return buf;
}

void WriteStringToOutputArchive(OutputArchive archive, const char* str) {
    auto s = std::string(str);
    GetArchive(archive)->Process(s);
}

Buffer ReadStringFromInputArchive(InputArchive archive) {
    auto s = std::string();
    GetArchive(archive)->Process(s);

    auto ret = AllocBuffer(static_cast<uint32_t>(s.size()));
    memcpy_s(ret.ptr, ret.size, s.c_str(), s.size());
    return ret;
}

// #include <cereal/types/vector.hpp>
//
// struct Testt {
// public:
//     int JJ;
//
//     Testt() {}
//     Testt(int i) : JJ(i) {  }
//
//     template <class Archive>
//     void serialize(Archive& ar) {
//         ar(CEREAL_NVP(JJ));
//     }
// };
//
// struct Data {
//     std::vector<Testt> vec = { 1, 2, 3, 4, 5 };
//
//     template <class Archive>
//     void serialize(Archive& ar) {
//         ar(vec);
//     }
// };
//
//
// void Test() {
//     std::stringstream ss;
//
//     // 序列化
//     {
//         cereal::JSONOutputArchive oarchive(ss);
//         Data data;
//         oarchive(data);
//     }
//
//     std::cout << ss.str() << std::endl;
//
//     // 反序列化
//     {
//         cereal::JSONInputArchive iarchive(ss);
//         Data data;
//         iarchive(data);
//         for (auto i : data.vec) {
//             std::cout << i.JJ << " ";
//         }
//         std::cout << std::endl;
//     }
// }
