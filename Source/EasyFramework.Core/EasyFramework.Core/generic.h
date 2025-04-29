#pragma once
#include "global.h"

#include <cstdint>
#include <exception>

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
    ERROR_CODE_NONE,
    ERROR_CODE_SERIALIZER_FAILED,
    ERROR_CODE_TEMPLATE_ENGINE_RENDER_FAILED,
    ERROR_CODE_UNKNOWN
};

EXPORT ErrorCode GetErrorCode();
EXPORT Buffer GetErrorMsg();

void SetErrorCode(ErrorCode ec);
void SetErrorMsg(const char* msg);

void HandleError(const std::exception& e);
void HandleNonStandardError();

#define TRY_CATCH_BEGIN             \
try {                               \
    SetErrorCode(ERROR_CODE_NONE);  \
    SetErrorMsg(nullptr);           \

#define TRY_CATCH_END               \
}                                   \
catch (const std::exception& e) {   \
    HandleError(e);                 \
}                                   \
catch (const std::bad_alloc& e) {   \
    throw e;                        \
}                                   \
catch (...) {                       \
    HandleNonStandardError();       \
}
