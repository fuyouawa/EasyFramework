#include "generic.h"
#include "stream_wrapper.hpp"

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

void WriteToIoStreamBuffer(IoStream stream, Buffer buffer) {
    auto s = GetStream(stream);
    s->stream()->write(buffer.ptr, buffer.size);
}

Buffer GetIoStreamBuffer(IoStream stream) {
    auto buf = GetStream(stream)->buffer();

    auto buffer = AllocBuffer(static_cast<uint32_t>(buf.size()));
    memcpy_s(buffer.ptr, buffer.size, buf.c_str(), buf.size());
    return buffer;
}

namespace {
ErrorCode last_error;
}

ErrorCode GetErrorCode() {
    return last_error;
}

void SetErrorCode(ErrorCode ec) {
    last_error = ec;
}
