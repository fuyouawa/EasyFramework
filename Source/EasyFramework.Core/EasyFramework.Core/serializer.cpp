#include "serializer.h"
#include "stream_wrapper.hpp"
#include "archive_wrapper.hpp"

OutputStreamWrapper* GetOutputStream(const OutputStream& stream) {
    return reinterpret_cast<OutputStreamWrapper*>(stream.ptr);
}

InputStreamWrapper* GetInputStream(const InputStream& stream) {
    return reinterpret_cast<InputStreamWrapper*>(stream.ptr);
}

OutputArchiveWrapper* GetOutputArchive(const OutputArchive& archive) {
    return reinterpret_cast<OutputArchiveWrapper*>(archive.ptr);
}

InputArchiveWrapper* GetInputArchive(const InputArchive& archive) {
    return reinterpret_cast<InputArchiveWrapper*>(archive.ptr);
}

OutputArchive AllocBinaryOutputArchive(OutputStream stream)
{
    auto archive = new BinaryOutputArchiveWrapper(*GetOutputStream(stream)->stream());
    auto ret = OutputArchive();
    ret.ptr = archive;
    return ret;
}

OutputStream AllocStringOutputStream()
{
    auto s = new StringOutputStreamWrapper();
    auto ret = OutputStream();
    ret.ptr = s;
    return ret;
}

void FreeOutputStream(OutputStream stream)
{
    delete GetOutputStream(stream);
}

InputStream AllocStringInputStream()
{
    auto s = new StringInputStreamWrapper();
    auto ret = InputStream();
    ret.ptr = s;
    return ret;
}

void FreeInputStream(InputStream stream)
{
    delete GetInputStream(stream);
}

size_t GetInputStreamSize(InputStream stream)
{
    return GetInputStream(stream)->size();
}

void ReadInputStream(InputStream stream, void* buffer, size_t buffer_size)
{
    auto str = GetInputStream(stream)->str();
    memcpy_s(buffer, buffer_size, str.c_str(), str.size());
}

void FreeOutputArchive(OutputArchive archive)
{
    delete GetOutputArchive(archive);
}

InputArchive AllocBinaryInputArchive(InputStream stream)
{
    auto archive = new BinaryInputArchiveWrapper(*GetInputStream(stream)->stream());
    auto ret = InputArchive();
    ret.ptr = archive;
    return ret;
}

void FreeInputArchive(InputArchive archive)
{
    delete GetInputArchive(archive);
}

void WriteToOutputArchive(OutputArchive archive, int value)
{
    GetOutputArchive(archive)->Process(value);
}

void ReadFromInputArchive(InputArchive archive, int& value)
{
    GetInputArchive(archive)->Process(value);
}


