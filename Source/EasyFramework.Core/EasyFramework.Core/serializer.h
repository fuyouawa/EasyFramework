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

EXPORT void WriteToIoStreamBuffer(IoStream stream, Buffer buffer);
EXPORT Buffer GetIoStreamBuffer(IoStream stream);


EXPORT OutputArchive AllocBinaryOutputArchive(IoStream stream);
EXPORT InputArchive AllocBinaryInputArchive(IoStream stream);

EXPORT OutputArchive AllocJsonOutputArchive(IoStream stream);
EXPORT InputArchive AllocJsonInputArchive(IoStream stream);

EXPORT void FreeOutputArchive(OutputArchive archive);
EXPORT void FreeInputArchive(InputArchive archive);


EXPORT void OutputArchiveSetNextName(OutputArchive archive, const char* name);
EXPORT void InputArchiveSetNextName(InputArchive archive, const char* name);

EXPORT void OutputArchiveStartNode(OutputArchive archive);
EXPORT void OutputArchiveFinishNode(OutputArchive archive);

EXPORT void InputArchiveStartNode(InputArchive archive);
EXPORT void InputArchiveFinishNode(InputArchive archive);

EXPORT void WriteInt64ToOutputArchive(OutputArchive archive, int64_t value);
EXPORT int64_t ReadInt64FromInputArchive(InputArchive archive);

EXPORT void WriteInt32ToOutputArchive(OutputArchive archive, int32_t value);
EXPORT int32_t ReadInt32FromInputArchive(InputArchive archive);

EXPORT void WriteInt16ToOutputArchive(OutputArchive archive, int16_t value);
EXPORT int16_t ReadInt16FromInputArchive(InputArchive archive);

EXPORT void WriteInt8ToOutputArchive(OutputArchive archive, int8_t value);
EXPORT int8_t ReadInt8FromInputArchive(InputArchive archive);


EXPORT void WriteUInt64ToOutputArchive(OutputArchive archive, uint64_t value);
EXPORT uint64_t ReadUInt64FromInputArchive(InputArchive archive);

EXPORT void WriteUInt32ToOutputArchive(OutputArchive archive, uint32_t value);
EXPORT uint32_t ReadUInt32FromInputArchive(InputArchive archive);

EXPORT void WriteUInt16ToOutputArchive(OutputArchive archive, uint16_t value);
EXPORT uint16_t ReadUInt16FromInputArchive(InputArchive archive);

EXPORT void WriteUInt8ToOutputArchive(OutputArchive archive, uint8_t value);
EXPORT uint8_t ReadUInt8FromInputArchive(InputArchive archive);



EXPORT void WriteVarint32ToOutputArchive(OutputArchive archive, uint32_t value);
EXPORT uint32_t ReadVarint32FromInputArchive(InputArchive archive);

EXPORT void WriteFloatToOutputArchive(OutputArchive archive, float value);
EXPORT float ReadFloatFromInputArchive(InputArchive archive);

EXPORT void WriteDoubleToOutputArchive(OutputArchive archive, double value);
EXPORT double ReadDoubleFromInputArchive(InputArchive archive);

EXPORT void WriteBinaryToOutputArchive(OutputArchive archive, Buffer buffer);
EXPORT Buffer ReadBinaryFromInputArchive(InputArchive archive);

EXPORT void WriteStringToOutputArchive(OutputArchive archive, const char* str);
EXPORT Buffer ReadStringFromInputArchive(InputArchive archive);
