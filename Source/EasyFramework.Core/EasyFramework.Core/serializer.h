#pragma once
#include <cstdint>

struct OutputStream
{
    void* ptr;
};

struct InputStream
{
    void* ptr;
};

struct OutputArchive
{
    void* ptr;
};

struct InputArchive
{
    void* ptr;
};

extern "C" __declspec(dllexport) OutputStream AllocStringOutputStream();
extern "C" __declspec(dllexport) void FreeOutputStream(OutputStream stream);

extern "C" __declspec(dllexport) InputStream AllocStringInputStream();
extern "C" __declspec(dllexport) void FreeInputStream(InputStream stream);

extern "C" __declspec(dllexport) size_t GetInputStreamSize(InputStream stream);
extern "C" __declspec(dllexport) void ReadInputStream(InputStream stream, void* buffer, size_t buffer_size);

extern "C" __declspec(dllexport) OutputArchive AllocBinaryOutputArchive(OutputStream stream);
extern "C" __declspec(dllexport) void FreeOutputArchive(OutputArchive archive);

extern "C" __declspec(dllexport) InputArchive AllocBinaryInputArchive(InputStream stream);
extern "C" __declspec(dllexport) void FreeInputArchive(InputArchive archive);

extern "C" __declspec(dllexport) void WriteToOutputArchive(OutputArchive archive, int value);
extern "C" __declspec(dllexport) void ReadFromInputArchive(InputArchive archive, int& value);
