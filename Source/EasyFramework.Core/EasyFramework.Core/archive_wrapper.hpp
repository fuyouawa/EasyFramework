#pragma once

#include "algorithm.hpp"

#include <cereal/archives/binary.hpp>
#include <cereal/archives/json.hpp>
#include <cereal/archives/xml.hpp>

struct Varint32 {
    uint32_t value;

    Varint32() : value() {}
    Varint32(uint32_t val) : value(val) { }

    template <class Archive>
    void serialize(Archive& ar) {
        if (Archive::is_saving::value) {
            std::vector<uint8_t> buffer;
            while (value >= 0x80) {
                buffer.push_back(static_cast<uint8_t>((value & 0x7F) | 0x80));
                value >>= 7;
            }
            buffer.push_back(static_cast<uint8_t>(value));
            ar(cereal::binary_data(buffer.data(), buffer.size()));
        }
        else {
            uint32_t result = 0;
            int shift = 0;
            while (true) {
                uint8_t byte;
                ar(byte);

                result |= (byte & 0x7F) << shift;
                if ((byte & 0x80) == 0) break;
                shift += 7;
            }
            value = result;
        }
    }
};

namespace cereal {
template<class Archive, class CharT, class Traits, class Alloc>
void CEREAL_SAVE_FUNCTION_NAME(Archive& ar, std::basic_string<CharT, Traits, Alloc> const& str)
    requires traits::is_output_serializable<BinaryData<CharT>, Archive>::value
{
    ar(Varint32(static_cast<uint32_t>(str.size())));
    ar(binary_data(str.data(), str.size() * sizeof(CharT)));
}

template<class Archive, class CharT, class Traits, class Alloc>
void CEREAL_LOAD_FUNCTION_NAME(Archive& ar, std::basic_string<CharT, Traits, Alloc>& str)
    requires traits::is_input_serializable<BinaryData<CharT>, Archive>::value
{
    auto size = Varint32();
    ar(size);
    str.resize(size.value);
    ar(binary_data(const_cast<CharT*>(str.data()), size.value * sizeof(CharT)));
}
}

#define ARCHIVE_PROCESS_DECL(type) virtual void ProcessImpl(type value) = 0
#define ARCHIVE_PROCESS_IMPL(type) void ProcessImpl(type value) override { archive_(value); }

#define ARCHIVE_PROCESS_NVP_DECL(type) ARCHIVE_PROCESS_DECL(cereal::NameValuePair<type>)
#define ARCHIVE_PROCESS_NVP_IMPL(type) ARCHIVE_PROCESS_IMPL(cereal::NameValuePair<type>)

enum class ArchiveType {
    Binary,
    Json,
    Xml
};

class ArchiveWrapper {
public:
    ArchiveWrapper(const ArchiveType type) : type_(type) {}
    virtual ~ArchiveWrapper() = default;

    ArchiveType type() const { return type_; }

    template<class T>
    void Process(const char* name, T& value) {
        if (name == nullptr || *name == '\0') {
            ProcessImpl(value);
        }
        else {
            ProcessImpl(cereal::make_nvp(name, value));
        }
    }

    void Process(cereal::SizeTag<size_t&> tag) {
        ProcessImpl(tag);
    }

    void Process(const char* name, cereal::BinaryData<char*&>& data) {
        if (name == nullptr || *name == '\0') {
            ProcessImpl(data);
        }
        else {
            ProcessImpl(cereal::make_nvp(name, data));
        }
    }

protected:
    ARCHIVE_PROCESS_DECL(cereal::SizeTag<size_t&>);

    ARCHIVE_PROCESS_DECL(int64_t&);
    ARCHIVE_PROCESS_DECL(int32_t&);
    ARCHIVE_PROCESS_DECL(int16_t&);
    ARCHIVE_PROCESS_DECL(int8_t&);
    ARCHIVE_PROCESS_DECL(uint64_t&);
    ARCHIVE_PROCESS_DECL(uint32_t&);
    ARCHIVE_PROCESS_DECL(uint16_t&);
    ARCHIVE_PROCESS_DECL(uint8_t&);
    ARCHIVE_PROCESS_DECL(float&);
    ARCHIVE_PROCESS_DECL(double&);
    ARCHIVE_PROCESS_DECL(Varint32&);
    ARCHIVE_PROCESS_DECL(std::string&);
    ARCHIVE_PROCESS_DECL(cereal::BinaryData<char*&>&);

    ARCHIVE_PROCESS_NVP_DECL(int64_t&);
    ARCHIVE_PROCESS_NVP_DECL(int32_t&);
    ARCHIVE_PROCESS_NVP_DECL(int16_t&);
    ARCHIVE_PROCESS_NVP_DECL(int8_t&);
    ARCHIVE_PROCESS_NVP_DECL(uint64_t&);
    ARCHIVE_PROCESS_NVP_DECL(uint32_t&);
    ARCHIVE_PROCESS_NVP_DECL(uint16_t&);
    ARCHIVE_PROCESS_NVP_DECL(uint8_t&);
    ARCHIVE_PROCESS_NVP_DECL(float&);
    ARCHIVE_PROCESS_NVP_DECL(double&);
    ARCHIVE_PROCESS_NVP_DECL(Varint32&);
    ARCHIVE_PROCESS_NVP_DECL(std::string&);
    ARCHIVE_PROCESS_NVP_DECL(cereal::BinaryData<char*&>&);

private:
    const ArchiveType type_;
};

class OutputArchiveWrapper : public ArchiveWrapper {
public:
    OutputArchiveWrapper(const ArchiveType type) : ArchiveWrapper(type) {}
    ~OutputArchiveWrapper() override = default;
};

class InputArchiveWrapper : public ArchiveWrapper {
public:
    InputArchiveWrapper(const ArchiveType type) : ArchiveWrapper(type) {}
    ~InputArchiveWrapper() override = default;
};

template<class Archive, uint32_t kFlags = 0>
class TemplateOutputArchiveWrapper : public OutputArchiveWrapper {
public:
    using OutputArchive = cereal::OutputArchive<Archive, kFlags>;

    TemplateOutputArchiveWrapper(const ArchiveType type, OutputArchive& archive)
        : OutputArchiveWrapper(type),
        archive_(archive) {}
    ~TemplateOutputArchiveWrapper() override = default;

protected:
    ARCHIVE_PROCESS_IMPL(cereal::SizeTag<size_t&>);

    ARCHIVE_PROCESS_IMPL(int64_t&);
    ARCHIVE_PROCESS_IMPL(int32_t&);
    ARCHIVE_PROCESS_IMPL(int16_t&);
    ARCHIVE_PROCESS_IMPL(int8_t&);
    ARCHIVE_PROCESS_IMPL(uint64_t&);
    ARCHIVE_PROCESS_IMPL(uint32_t&);
    ARCHIVE_PROCESS_IMPL(uint16_t&);
    ARCHIVE_PROCESS_IMPL(uint8_t&);
    ARCHIVE_PROCESS_IMPL(float&);
    ARCHIVE_PROCESS_IMPL(double&);
    ARCHIVE_PROCESS_IMPL(Varint32&);
    ARCHIVE_PROCESS_IMPL(std::string&);
    ARCHIVE_PROCESS_IMPL(cereal::BinaryData<char*&>&);

    ARCHIVE_PROCESS_NVP_IMPL(int64_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int32_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int16_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int8_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint64_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint32_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint16_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint8_t&);
    ARCHIVE_PROCESS_NVP_IMPL(float&);
    ARCHIVE_PROCESS_NVP_IMPL(double&);
    ARCHIVE_PROCESS_NVP_IMPL(Varint32&);
    ARCHIVE_PROCESS_NVP_IMPL(std::string&);
    ARCHIVE_PROCESS_NVP_IMPL(cereal::BinaryData<char*&>&);

private:
    OutputArchive& archive_;
};


template<class Archive, uint32_t kFlags = 0>
class TemplateInputArchiveWrapper : public InputArchiveWrapper {
public:
    using InputArchive = cereal::InputArchive<Archive, kFlags>;

    TemplateInputArchiveWrapper(const ArchiveType type, InputArchive& archive)
        : InputArchiveWrapper(type),
        archive_(archive) {}
    ~TemplateInputArchiveWrapper() override = default;

protected:
    ARCHIVE_PROCESS_IMPL(cereal::SizeTag<size_t&>);

    ARCHIVE_PROCESS_IMPL(int64_t&);
    ARCHIVE_PROCESS_IMPL(int32_t&);
    ARCHIVE_PROCESS_IMPL(int16_t&);
    ARCHIVE_PROCESS_IMPL(int8_t&);
    ARCHIVE_PROCESS_IMPL(uint64_t&);
    ARCHIVE_PROCESS_IMPL(uint32_t&);
    ARCHIVE_PROCESS_IMPL(uint16_t&);
    ARCHIVE_PROCESS_IMPL(uint8_t&);
    ARCHIVE_PROCESS_IMPL(float&);
    ARCHIVE_PROCESS_IMPL(double&);
    ARCHIVE_PROCESS_IMPL(Varint32&);
    ARCHIVE_PROCESS_IMPL(std::string&);
    ARCHIVE_PROCESS_IMPL(cereal::BinaryData<char*&>&);

    ARCHIVE_PROCESS_NVP_IMPL(int64_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int32_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int16_t&);
    ARCHIVE_PROCESS_NVP_IMPL(int8_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint64_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint32_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint16_t&);
    ARCHIVE_PROCESS_NVP_IMPL(uint8_t&);
    ARCHIVE_PROCESS_NVP_IMPL(float&);
    ARCHIVE_PROCESS_NVP_IMPL(double&);
    ARCHIVE_PROCESS_NVP_IMPL(Varint32&);
    ARCHIVE_PROCESS_NVP_IMPL(std::string&);
    ARCHIVE_PROCESS_NVP_IMPL(cereal::BinaryData<char*&>&);

private:
    InputArchive& archive_;
};

class BinaryOutputArchiveWrapper : TemplateOutputArchiveWrapper<cereal::BinaryOutputArchive, cereal::AllowEmptyClassElision> {
public:
    BinaryOutputArchiveWrapper(std::ostream& stream)
        : TemplateOutputArchiveWrapper(ArchiveType::Binary, archive_),
        archive_(stream)
    {}

private:
    cereal::BinaryOutputArchive archive_;
};

class BinaryInputArchiveWrapper : TemplateInputArchiveWrapper<cereal::BinaryInputArchive, cereal::AllowEmptyClassElision> {
public:
    BinaryInputArchiveWrapper(std::istream& stream)
        : TemplateInputArchiveWrapper(ArchiveType::Binary, archive_),
        archive_(stream)
    {}

private:
    cereal::BinaryInputArchive archive_;
};
