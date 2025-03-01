#include "serializer.h"
#include <iostream>

int main() {
    auto ios = AllocStringIoStream();
    auto oarch = AllocBinaryOutputArchive(ios);
    WriteInt32ToOutputArchive(oarch, "num", 123);

    WriteStringToOutputArchive(oarch, "str", "777");

    auto rrr = GetIoStreamBuffer(ios);

    auto iarch = AllocBinaryInputArchive(ios);
    auto val = ReadInt32FromInputArchive(iarch, "num");

    auto str = ReadStringFromInputArchive(iarch, "str");

    std::cout << val << std::endl;
}
