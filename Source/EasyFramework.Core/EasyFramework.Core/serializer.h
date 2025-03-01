#pragma once
#include "global.h"

#include <cstdint>

struct IoStream
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

struct Buffer {
    char* ptr;
    uint32_t size;
};


EXPORT Buffer AllocBuffer(uint32_t size);
EXPORT void FreeBuffer(Buffer buffer);

EXPORT IoStream AllocStringIoStream();
EXPORT void FreeIoStream(IoStream stream);

EXPORT Buffer GetIoStreamBuffer(IoStream stream);

EXPORT OutputArchive AllocBinaryOutputArchive(IoStream stream);
EXPORT void FreeOutputArchive(OutputArchive archive);

EXPORT InputArchive AllocBinaryInputArchive(IoStream stream);
EXPORT void FreeInputArchive(InputArchive archive);



EXPORT void WriteInt64ToOutputArchive(OutputArchive archive, const char* name, int64_t value);
EXPORT int64_t ReadInt64FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteInt32ToOutputArchive(OutputArchive archive, const char* name, int32_t value);
EXPORT int32_t ReadInt32FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteInt16ToOutputArchive(OutputArchive archive, const char* name, int16_t value);
EXPORT int16_t ReadInt16FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteInt8ToOutputArchive(OutputArchive archive, const char* name, int8_t value);
EXPORT int8_t ReadInt8FromInputArchive(InputArchive archive, const char* name);


EXPORT void WriteUInt64ToOutputArchive(OutputArchive archive, const char* name, uint64_t value);
EXPORT uint64_t ReadUInt64FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteUInt32ToOutputArchive(OutputArchive archive, const char* name, uint32_t value);
EXPORT uint32_t ReadUInt32FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteUInt16ToOutputArchive(OutputArchive archive, const char* name, uint16_t value);
EXPORT uint16_t ReadUInt16FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteUInt8ToOutputArchive(OutputArchive archive, const char* name, uint8_t value);
EXPORT uint8_t ReadUInt8FromInputArchive(InputArchive archive, const char* name);



EXPORT void WriteVarint32ToOutputArchive(OutputArchive archive, const char* name, uint32_t value);
EXPORT uint32_t ReadVarint32FromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteFloatToOutputArchive(OutputArchive archive, const char* name, float value);
EXPORT float ReadFloatFromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteDoubleToOutputArchive(OutputArchive archive, const char* name, double value);
EXPORT double ReadDoubleFromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteBinaryToOutputArchive(OutputArchive archive, const char* name, Buffer buffer);
EXPORT Buffer ReadBinaryFromInputArchive(InputArchive archive, const char* name);

EXPORT void WriteStringToOutputArchive(OutputArchive archive, const char* name, const char* str);
EXPORT Buffer ReadStringFromInputArchive(InputArchive archive, const char* name);
