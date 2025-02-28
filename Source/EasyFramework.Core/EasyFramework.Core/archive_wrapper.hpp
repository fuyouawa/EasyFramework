#pragma once

#include <cereal/archives/binary.hpp>
#include <cereal/archives/xml.hpp>

class ArchiveWrapper
{
public:
    ArchiveWrapper() {}
    virtual ~ArchiveWrapper() {}
};

class OutputArchiveWrapper : public ArchiveWrapper
{
public:
    OutputArchiveWrapper() {}
    virtual ~OutputArchiveWrapper() {}

    virtual void Process(int value) = 0;
};

class InputArchiveWrapper : public ArchiveWrapper
{
public:
    InputArchiveWrapper() {}
    virtual ~InputArchiveWrapper() {}

    virtual void Process(int& value) = 0;
};

class BinaryOutputArchiveWrapper : public OutputArchiveWrapper
{
public:
    BinaryOutputArchiveWrapper(std::ostream& stream)
        : arch_(stream)
    {}

    ~BinaryOutputArchiveWrapper() override {}

    void Process(int value) override {
        arch_(value);
    }

private:
    cereal::BinaryOutputArchive arch_;
};

class BinaryInputArchiveWrapper : public InputArchiveWrapper
{
public:
    BinaryInputArchiveWrapper(std::istream& stream)
        : arch_(stream)
    {
    }

    ~BinaryInputArchiveWrapper() override {}

    void Process(int& value) override {
        arch_(value);
    }

private:
    cereal::BinaryInputArchive arch_;
};
