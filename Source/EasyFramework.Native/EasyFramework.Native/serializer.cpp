#include "serializer.h"
#include "stream_wrapper.hpp"
#include "archive_wrapper.hpp"

OutputArchive AllocBinaryOutputArchive(IoStream stream) {
    TRY_CATCH_BEGIN
        auto archive = new BinaryOutputArchiveWrapper(*GetStream(stream)->stream());
        auto ret = OutputArchive();
        ret.ptr = archive;
        return ret;
    TRY_CATCH_END
    return {};
}

void FreeOutputArchive(OutputArchive archive) {
    TRY_CATCH_BEGIN
        delete GetArchive(archive);
    TRY_CATCH_END
}

InputArchive AllocBinaryInputArchive(IoStream stream) {
    TRY_CATCH_BEGIN
        auto archive = new BinaryInputArchiveWrapper(*GetStream(stream)->stream());
        auto ret = InputArchive();
        ret.ptr = archive;
        return ret;
    TRY_CATCH_END
    return {};
}

OutputArchive AllocJsonOutputArchive(IoStream stream) {
    TRY_CATCH_BEGIN
        auto archive = new JsonOutputArchiveWrapper(*GetStream(stream)->stream());
        auto ret = OutputArchive();
        ret.ptr = archive;
        return ret;
    TRY_CATCH_END
    return {};
}

InputArchive AllocJsonInputArchive(IoStream stream) {
    TRY_CATCH_BEGIN
        auto archive = new JsonInputArchiveWrapper(*GetStream(stream)->stream());
        auto ret = InputArchive();
        ret.ptr = archive;
        return ret;
    TRY_CATCH_END
    return {};
}

void FreeInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
        delete GetArchive(archive);
    TRY_CATCH_END
}

void OutputArchiveSetNextName(OutputArchive archive, const char* name) {
    TRY_CATCH_BEGIN
        auto str = std::string();
        if (name != nullptr) {
            str = name;
        }
        GetArchive(archive)->set_next_name(std::move(str));
    TRY_CATCH_END
}

void InputArchiveSetNextName(InputArchive archive, const char* name) {
    TRY_CATCH_BEGIN
        auto str = std::string();
        if (name != nullptr) {
            str = name;
        }
        GetArchive(archive)->set_next_name(std::move(str));
    TRY_CATCH_END
}

void OutputArchiveStartNode(OutputArchive archive) {
    TRY_CATCH_BEGIN
        GetArchive(archive)->StartNode();
    TRY_CATCH_END
}

void OutputArchiveFinishNode(OutputArchive archive) {
    TRY_CATCH_BEGIN
        GetArchive(archive)->FinishNode();
    TRY_CATCH_END
}

void InputArchiveStartNode(InputArchive archive) {
    TRY_CATCH_BEGIN
        GetArchive(archive)->StartNode();
    TRY_CATCH_END
}

void InputArchiveFinishNode(InputArchive archive) {
    TRY_CATCH_BEGIN
        GetArchive(archive)->FinishNode();
    TRY_CATCH_END
}

void WriteSizeToOutputArchive(OutputArchive archive, uint32_t size) {
    TRY_CATCH_BEGIN
        auto arch = GetArchive(archive);
        if (arch->type() == ArchiveType::Binary) {
            auto v = Varint32(size);
            arch->Process(v);
        }
        else {
            auto s = static_cast<uint64_t>(size);
            arch->Process(cereal::make_size_tag(s));
        }
    TRY_CATCH_END
}

uint32_t ReadSizeFromInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
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
    TRY_CATCH_END
    return {};
}

#define NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(type_name, type) \
void Write##type_name##ToOutputArchive(OutputArchive archive, type value) { \
    TRY_CATCH_BEGIN                         \
    GetArchive(archive)->Process(value);    \
    TRY_CATCH_END                           \
}                                           \
type Read##type_name##FromInputArchive(InputArchive archive) { \
    TRY_CATCH_BEGIN                         \
    auto ret = type();                      \
    GetArchive(archive)->Process(ret);      \
    return ret;                             \
    TRY_CATCH_END                           \
    return {};                              \
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
    TRY_CATCH_BEGIN
        auto b = value != 0;
        GetArchive(archive)->Process(b);
    TRY_CATCH_END
}

uint8_t ReadBoolFromInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
        auto b = bool();
        GetArchive(archive)->Process(b);
        return b ? 1 : 0;
    TRY_CATCH_END
    return {};
}

NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Float, float)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Double, double)

void WriteVarint32ToOutputArchive(OutputArchive archive, uint32_t value) {
    TRY_CATCH_BEGIN
        auto arch = GetArchive(archive);
        if (arch->type() != ArchiveType::Binary) {
            arch->Process(value);
        }
        else {
            auto v = Varint32(value);
            arch->Process(v);
        }
    TRY_CATCH_END
}

uint32_t ReadVarint32FromInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
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
    TRY_CATCH_END
    return {};
}

void WriteBinaryToOutputArchive(OutputArchive archive, Buffer buffer) {
    TRY_CATCH_BEGIN
        auto arch = GetArchive(archive);
        if (arch->type() == ArchiveType::Binary) {
            auto v = Varint32(buffer.size);
            arch->Process(v);
        }
        auto data = std::vector<uint8_t>(buffer.ptr, buffer.ptr + buffer.size);
        arch->Process(data);
    TRY_CATCH_END
}

Buffer ReadBinaryFromInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
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
    TRY_CATCH_END
    return {};
}

void WriteStringToOutputArchive(OutputArchive archive, const char* str) {
    TRY_CATCH_BEGIN
        auto s = std::string();
        if (str) {
            s = str;
        }
        GetArchive(archive)->Process(s);
    TRY_CATCH_END
}

Buffer ReadStringFromInputArchive(InputArchive archive) {
    TRY_CATCH_BEGIN
        auto s = std::string();
        GetArchive(archive)->Process(s);

        auto ret = AllocBuffer(static_cast<uint32_t>(s.size()));
        memcpy_s(ret.ptr, ret.size, s.c_str(), s.size());
        return ret;
    TRY_CATCH_END
    return {};
}
