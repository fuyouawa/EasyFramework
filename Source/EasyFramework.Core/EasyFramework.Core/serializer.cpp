#include "serializer.h"
#include "stream_wrapper.hpp"
#include "archive_wrapper.hpp"

namespace {
IoStreamWrapper* GetStream(const IoStream& stream) {
    return reinterpret_cast<IoStreamWrapper*>(stream.ptr);
}

OutputArchiveWrapper* GetArchive(const OutputArchive& archive) {
    return reinterpret_cast<OutputArchiveWrapper*>(archive.ptr);
}

InputArchiveWrapper* GetArchive(const InputArchive& archive) {
    return reinterpret_cast<InputArchiveWrapper*>(archive.ptr);
}
}

OutputArchive AllocBinaryOutputArchive(IoStream stream) {
    auto archive = new BinaryOutputArchiveWrapper(*GetStream(stream)->stream());
    auto ret = OutputArchive();
    ret.ptr = archive;
    return ret;
}

Buffer AllocBuffer(uint32_t size) {
    auto buf = Buffer();
    buf.ptr = new char[size];
    buf.size = size;
    return buf;
}

void FreeBuffer(Buffer buffer) {
    delete[] buffer.ptr;
}

IoStream AllocStringIoStream() {
    auto s = new StringIoStreamWrapper({});
    auto ret = IoStream();
    ret.ptr = s;
    return ret;
}

void FreeIoStream(IoStream stream) {
    delete GetStream(stream);
}

Buffer GetIoStreamBuffer(IoStream stream) {
    auto buf = GetStream(stream)->buffer();

    auto buffer = AllocBuffer(static_cast<uint32_t>(buf.size()));
    memcpy_s(buffer.ptr, buffer.size, buf.c_str(), buf.size());
    return buffer;
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

void FreeInputArchive(InputArchive archive) {
    delete GetArchive(archive);
}

#define NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(type_name, type) \
void Write##type_name##ToOutputArchive(OutputArchive archive, const char* name, type value) { \
    GetArchive(archive)->Process(name, value); \
} \
type Read##type_name##FromInputArchive(InputArchive archive, const char* name) { \
    auto ret = type(); \
    GetArchive(archive)->Process(name, ret); \
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

NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Float, float)
NORMAL_WRITE_TO_OUTPUT_ARCHIVE_IMPL(Double, double)

void WriteVarint32ToOutputArchive(OutputArchive archive, const char* name, uint32_t value) {
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        arch->Process(name, value);
    }
    else {
        auto v = Varint32(value);
        arch->Process(name, v);
    }
}

uint32_t ReadVarint32FromInputArchive(InputArchive archive, const char* name) {
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        uint32_t ret;
        arch->Process(name, ret);
        return ret;
    }
    else {
        auto ret = Varint32();
        arch->Process(name, ret);
        return ret.value;
    }
}

void WriteBinaryToOutputArchive(OutputArchive archive, const char* name, Buffer buffer) {
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        auto s = static_cast<size_t>(buffer.size);
        arch->Process(cereal::make_size_tag(s));
    }
    else {
        auto v = Varint32(buffer.size);
        arch->Process(name, v);
    }
    auto data = cereal::binary_data(buffer.ptr, buffer.size);
    arch->Process(name, data);
}

Buffer ReadBinaryFromInputArchive(InputArchive archive, const char* name) {
    auto size = size_t();
    auto arch = GetArchive(archive);
    if (arch->type() != ArchiveType::Binary) {
        arch->Process(cereal::make_size_tag(size));
    }
    else {
        auto v = Varint32();
        arch->Process(name, v);
        size = v.value;
    }
    auto buf = AllocBuffer(static_cast<uint32_t>(size));
    auto data = cereal::binary_data(buf.ptr, buf.size);
    GetArchive(archive)->Process(name, data);
    return buf;
}

void WriteStringToOutputArchive(OutputArchive archive, const char* name, const char* str) {
    auto s = std::string(str);
    GetArchive(archive)->Process(name, s);
}

Buffer ReadStringFromInputArchive(InputArchive archive, const char* name) {
    auto s = std::string();
    GetArchive(archive)->Process(name, s);

    auto ret = AllocBuffer(static_cast<uint32_t>(s.size()));
    memcpy_s(ret.ptr, ret.size, s.c_str(), s.size());
    return ret;
}
