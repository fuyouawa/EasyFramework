#include "serializer.h"
#include "template_engine.h"
#include <iostream>

int main() {
    auto ios = AllocStringIoStream();
    //     auto env = AllocTemplateEngineEnvironment();
    //
    //     std::string template_str = R"(
    //     public class {{ class_name }} {
    //         {% if has_id %}
    //         public int Id { get; set; }
    //         {% endif %}
    //         public string Name { get; set; }
    //     }
    //     )";
    //
    //     RenderTemplateToStream(ios, env, template_str.c_str(), R"(
    //     {
    //         "jj" : false,
    //         "class_names" : "Test",
    //         "has_id" : true
    //     }
    // )");


    auto oarch = AllocJsonOutputArchive(ios);

    OutputArchiveStartNode(oarch);
    WriteSizeToOutputArchive(oarch, 3);

    WriteInt32ToOutputArchive(oarch, 134);
    WriteInt32ToOutputArchive(oarch, 35434);
    WriteInt32ToOutputArchive(oarch, 1356747);

    OutputArchiveFinishNode(oarch);

    // OutputArchiveSetNextName(oarch, "num");
    // WriteInt32ToOutputArchive(oarch, 123);
    //
    // OutputArchiveSetNextName(oarch, "test");
    // OutputArchiveStartNode(oarch);
    //
    // OutputArchiveSetNextName(oarch, "float");
    // WriteFloatToOutputArchive(oarch, 1.234f);

    OutputArchiveSetNextName(oarch, "Bool");
    WriteBoolToOutputArchive(oarch, 1);

    // OutputArchiveSetNextName(oarch, "str");
    // WriteStringToOutputArchive(oarch, "777");
    //
    // OutputArchiveSetNextName(oarch, "float");
    // auto bbb = AllocBuffer(6);
    // memcpy_s(bbb.ptr, bbb.size, "\x23\x33\x45\x6D\xAB", 6);
    // WriteBinaryToOutputArchive(oarch, bbb);
    //
    // OutputArchiveFinishNode(oarch);

    FreeOutputArchive(oarch);

    auto ios_buffer = GetIoStreamBuffer(ios);
    auto ios_str = std::string(ios_buffer.ptr, ios_buffer.size);
    std::cout << ios_str << std::endl;
}
