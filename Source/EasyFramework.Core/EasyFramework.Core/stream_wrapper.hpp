#pragma once
#include <sstream>

class StreamWrapper
{
public:
    StreamWrapper() {}
    virtual ~StreamWrapper() {}
};

class OutputStreamWrapper : StreamWrapper
{
public:
    OutputStreamWrapper() {}
    virtual ~OutputStreamWrapper() {}

    virtual std::ostream* stream() = 0;
};

class InputStreamWrapper : StreamWrapper
{
public:
    InputStreamWrapper() {}
    virtual ~InputStreamWrapper() {}

    virtual size_t size() = 0;
    virtual std::string str() = 0;
    virtual std::istream* stream() = 0;
};

class StringOutputStreamWrapper : OutputStreamWrapper
{
public:
    StringOutputStreamWrapper() {}
    ~StringOutputStreamWrapper() override {}

    std::ostream* stream() override {
        return &sstream_;
    }

private:
    std::ostringstream sstream_;
};


class StringInputStreamWrapper : InputStreamWrapper
{
public:
    StringInputStreamWrapper() {}
    ~StringInputStreamWrapper() override {}

    size_t size() override {
        return sstream_.str().size();
    }

    std::string str() override {
        return sstream_.str();
    }

    std::istream* stream() override {
        return &sstream_;
    }

private:
    std::istringstream sstream_;
};
