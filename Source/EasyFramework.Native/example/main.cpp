#include "serializer.h"
#include "template_engine.h"
#include <iostream>
#include <fcntl.h>
#include <io.h>
#include <Windows.h>

int main() {
    SetConsoleOutputCP(CP_UTF8);  // 设置输出为 UTF-8
    SetConsoleCP(CP_UTF8);        // 设置输入为 UTF-8
    _setmode(_fileno(stdin), _O_U8TEXT);
    _setmode(_fileno(stderr), _O_U8TEXT);

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

    OutputArchiveSetNextName(oarch, u8"345");
    OutputArchiveStartNode(oarch);

    WriteSizeToOutputArchive(oarch, 3);
    // OutputArchiveStartNode(oarch);

    WriteInt32ToOutputArchive(oarch, 134);
    WriteInt32ToOutputArchive(oarch, 35434);
    WriteInt32ToOutputArchive(oarch, 1356747);

    // OutputArchiveFinishNode(oarch);

    OutputArchiveFinishNode(oarch);

    // OutputArchiveSetNextName(oarch, "num");
    // WriteInt32ToOutputArchive(oarch, 123);
    //
    // OutputArchiveSetNextName(oarch, "test");
    // OutputArchiveStartNode(oarch);
    //
    // OutputArchiveSetNextName(oarch, "float");
    // WriteFloatToOutputArchive(oarch, 1.234f);

    // OutputArchiveSetNextName(oarch, u8"sefs阿松大");
    // WriteBoolToOutputArchive(oarch, 1);

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

    // auto iarch = AllocJsonInputArchive(ios);
    // InputArchiveStartNode(iarch);
    // auto size = ReadSizeFromInputArchive(iarch);
    // auto i32 = ReadInt32FromInputArchive(iarch);
    // auto i32_2 = ReadInt32FromInputArchive(iarch);
    // auto i32_3 = ReadInt32FromInputArchive(iarch);
    // InputArchiveFinishNode(iarch);
}
