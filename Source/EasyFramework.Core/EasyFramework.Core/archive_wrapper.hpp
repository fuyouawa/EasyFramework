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

    void set_next_name(std::string&& str) {
        if (type() == ArchiveType::Binary)
            return;
        next_name_ = std::move(str);
    }

    template<class T>
    void Process(T& value) {
        ProcessImpl(value);
    }

    virtual void StartNode() {}
    virtual void FinishNode() {}

protected:
    std::string PeekName() {
        if (next_name_.empty()) {
            return {};
        }
        auto tmp = std::string();
        tmp.swap(next_name_);
        return tmp;
    }

    virtual void ProcessImpl(int64_t& value) = 0;
    virtual void ProcessImpl(int32_t& value) = 0;
    virtual void ProcessImpl(int16_t& value) = 0;
    virtual void ProcessImpl(int8_t& value) = 0;
    virtual void ProcessImpl(uint64_t& value) = 0;
    virtual void ProcessImpl(uint32_t& value) = 0;
    virtual void ProcessImpl(uint16_t& value) = 0;
    virtual void ProcessImpl(uint8_t& value) = 0;
    virtual void ProcessImpl(float& value) = 0;
    virtual void ProcessImpl(double& value) = 0;
    virtual void ProcessImpl(Varint32& value) = 0;
    virtual void ProcessImpl(std::string& str) = 0;
    virtual void ProcessImpl(std::vector<uint8_t>& data) = 0;

private:
    const ArchiveType type_;
    std::string next_name_;
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
    void ProcessImpl(int64_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int32_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int16_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int8_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint64_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint32_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint16_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint8_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(float& value) override { AutoProcessImpl(value); }
    void ProcessImpl(double& value) override { AutoProcessImpl(value); }

    template<class T>
    void AutoProcessImpl(T& value) {
        if (type() == ArchiveType::Binary) {
            archive_(value);
            return;
        }

        auto name = PeekName();
        if (name.empty()) {
            archive_(value);
        }
        else {
            archive_(cereal::make_nvp(name, value));
        }
    }

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
    void ProcessImpl(int64_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int32_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int16_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(int8_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint64_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint32_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint16_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(uint8_t& value) override { AutoProcessImpl(value); }
    void ProcessImpl(float& value) override { AutoProcessImpl(value); }
    void ProcessImpl(double& value) override { AutoProcessImpl(value); }

    template<class T>
    void AutoProcessImpl(T& value) {
        auto name = PeekName();
        if (name.empty()) {
            archive_(value);
        }
        else {
            archive_(cereal::make_nvp(name, value));
        }
    }

private:
    InputArchive& archive_;
};

class BinaryOutputArchiveWrapper : public TemplateOutputArchiveWrapper<cereal::BinaryOutputArchive, cereal::AllowEmptyClassElision> {
public:
    BinaryOutputArchiveWrapper(std::ostream& stream)
        : TemplateOutputArchiveWrapper(ArchiveType::Binary, archive_),
        archive_(stream)
    {}

protected:
    void ProcessImpl(Varint32& value) override { archive_(value); }

    void ProcessImpl(std::string& str) override {
        archive_(Varint32(static_cast<uint32_t>(str.size())));
        archive_(cereal::binary_data(str.data(), str.size()));
    }

    void ProcessImpl(std::vector<uint8_t>& data) override {
        archive_(Varint32(static_cast<uint32_t>(data.size())));
        archive_(cereal::binary_data(data.data(), data.size()));
    }

private:
    cereal::BinaryOutputArchive archive_;
};

class BinaryInputArchiveWrapper : public TemplateInputArchiveWrapper<cereal::BinaryInputArchive, cereal::AllowEmptyClassElision> {
public:
    BinaryInputArchiveWrapper(std::istream& stream)
        : TemplateInputArchiveWrapper(ArchiveType::Binary, archive_),
        archive_(stream)
    {}

protected:
    void ProcessImpl(Varint32& value) override { archive_(value); }

    void ProcessImpl(std::string& str) override {
        auto size = Varint32();
        archive_(size);
        str.resize(size.value);
        archive_(cereal::binary_data(str.data(), size.value));
    }

    void ProcessImpl(std::vector<uint8_t>& data) override {
        auto size = Varint32();
        archive_(size);
        data.resize(size.value);
        archive_(cereal::binary_data(data.data(), size.value));
    }

private:
    cereal::BinaryInputArchive archive_;
};


class JsonOutputArchiveWrapper : public TemplateOutputArchiveWrapper<cereal::JSONOutputArchive> {
public:
    JsonOutputArchiveWrapper(std::ostream& stream)
        : TemplateOutputArchiveWrapper(ArchiveType::Json, archive_),
        archive_(stream)
    {
    }

    void StartNode() override {
        auto name = PeekName();
        if (!name.empty()) {
            archive_.setNextName(name.c_str());
        }
        archive_.startNode();
    }

    void FinishNode() override {
        archive_.finishNode();
    }

protected:
    void ProcessImpl(Varint32& value) override { AutoProcessImpl(value.value); }

    void ProcessImpl(std::string& str) override {
        AutoProcessImpl(str);
    }

    void ProcessImpl(std::vector<uint8_t>& value) override {
        auto str = EncodeBase64(value.data(), value.size());
        AutoProcessImpl(str);
    }

private:
    cereal::JSONOutputArchive archive_;
};

class JsonInputArchiveWrapper : public TemplateInputArchiveWrapper<cereal::JSONInputArchive> {
public:
    JsonInputArchiveWrapper(std::istream& stream)
        : TemplateInputArchiveWrapper(ArchiveType::Json, archive_),
        archive_(stream)
    {
    }

    void StartNode() override {
        auto name = PeekName();
        if (!name.empty()) {
            archive_.setNextName(name.c_str());
        }
        archive_.startNode();
    }

    void FinishNode() override {
        archive_.finishNode();
    }

protected:
    void ProcessImpl(Varint32& value) override { AutoProcessImpl(value.value); }

    void ProcessImpl(std::string& str) override {
        AutoProcessImpl(str);
    }

    void ProcessImpl(std::vector<uint8_t>& value) override {
        std::string str;
        AutoProcessImpl(str);
        value = DecodeBase64(str);
    }

private:
    cereal::JSONInputArchive archive_;
};
