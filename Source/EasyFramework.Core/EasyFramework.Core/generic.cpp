#include "generic.h"
#include "stream_wrapper.hpp"

#include <string>

Buffer AllocBuffer(uint32_t size) {
    auto ret = Buffer();
    ret.ptr = new char[size];
    ret.size = size;
    return ret;
}

void FreeBuffer(Buffer buffer) {
    delete[] buffer.ptr;
}

IoStream AllocStringIoStream() {
    TRY_CATCH_BEGIN
        auto s = new StringIoStreamWrapper({});
        auto ret = IoStream();
        ret.ptr = s;
        return ret;
    TRY_CATCH_END
    return {};
}

void FreeIoStream(IoStream stream) {
    delete GetStream(stream);
}

void WriteToIoStreamBuffer(IoStream stream, Buffer buffer) {
    TRY_CATCH_BEGIN
        auto s = GetStream(stream);
        s->stream()->write(buffer.ptr, buffer.size);
    TRY_CATCH_END
}

Buffer GetIoStreamBuffer(IoStream stream) {
    TRY_CATCH_BEGIN
        auto buf = GetStream(stream)->buffer();

        auto ret = AllocBuffer(static_cast<uint32_t>(buf.size()));
        memcpy_s(ret.ptr, ret.size, buf.c_str(), buf.size());
        return ret;
    TRY_CATCH_END
    return {};
}

namespace {
    ErrorCode last_error;
    std::string error_msg;
}

ErrorCode GetErrorCode() {
    return last_error;
}

Buffer GetErrorMsg() {
    if (error_msg.empty()) {
        return {};
    }
    auto ret = AllocBuffer(static_cast<uint32_t>(error_msg.size()));
    memcpy_s(ret.ptr, ret.size, error_msg.c_str(), error_msg.size());
    return ret;
}

void SetErrorCode(ErrorCode ec) {
    last_error = ec;
}

void SetErrorMsg(const char* msg) {
    if (!msg) {
        error_msg.clear();
    }
    else {
        error_msg = msg;
    }
}

#include <cereal/cereal.hpp>
#include <inja/exceptions.hpp>

void HandleError(const std::exception& e) {
    if (auto ce = dynamic_cast<const cereal::Exception*>(&e)) {
        SetErrorCode(ERROR_CODE_SERIALIZER_FAILED);
    }
    else if (auto ie = dynamic_cast<const inja::InjaError*>(&e)) {
        SetErrorCode(ERROR_CODE_TEMPLATE_ENGINE_RENDER_FAILED);
    }
    else {
        SetErrorCode(ERROR_CODE_UNKNOWN);
    }
    SetErrorMsg(e.what());
}

void HandleNonStandardError() {
    SetErrorCode(ERROR_CODE_UNKNOWN);
    SetErrorMsg(nullptr);
}
