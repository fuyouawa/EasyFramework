#pragma once
#include <sstream>

class StreamWrapper
{
public:
    StreamWrapper() = default;
    virtual ~StreamWrapper() = default;
};

class IoStreamWrapper : StreamWrapper
{
public:
    IoStreamWrapper() = default;
    ~IoStreamWrapper() override = default;

    virtual size_t size() = 0;
    virtual std::string buffer() = 0;
    virtual std::iostream* stream() = 0;
};


class StringIoStreamWrapper : IoStreamWrapper
{
public:
    StringIoStreamWrapper(const std::string& str) : sstream_(str) {}
    ~StringIoStreamWrapper() override = default;

    size_t size() override {
        return sstream_.str().size();
    }

    std::string buffer() override {
        return sstream_.str();
    }

    std::iostream* stream() override {
        return &sstream_;
    }

private:
    std::stringstream sstream_;
};

inline IoStreamWrapper* GetStream(const IoStream& stream) {
    return reinterpret_cast<IoStreamWrapper*>(stream.ptr);
}
