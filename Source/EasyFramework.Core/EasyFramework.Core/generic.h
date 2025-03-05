#pragma once
#include "global.h"

#include <cstdint>

struct Buffer {
    char* ptr;
    uint32_t size;
};

struct IoStream {
    void* ptr;
};


EXPORT Buffer AllocBuffer(uint32_t size);
EXPORT void FreeBuffer(Buffer buffer);

EXPORT IoStream AllocStringIoStream();
EXPORT void FreeIoStream(IoStream stream);

EXPORT void WriteToIoStreamBuffer(IoStream stream, Buffer buffer);
EXPORT Buffer GetIoStreamBuffer(IoStream stream);


enum ErrorCode {
    None,
    UnKnow
};

EXPORT ErrorCode GetErrorCode();

void SetErrorCode(ErrorCode ec);
