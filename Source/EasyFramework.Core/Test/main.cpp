#include "serializer.h"
#include <iostream>

int main() {
    auto ios = AllocStringIoStream();
    auto oarch = AllocBinaryOutputArchive(ios);

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

    auto rrr = GetIoStreamBuffer(ios);
    auto strr = std::string(rrr.ptr, rrr.size);
    std::cout << strr << std::endl;
}
