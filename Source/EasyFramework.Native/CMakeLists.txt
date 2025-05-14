cmake_minimum_required(VERSION 3.10)
project(EasyFramework.Native VERSION 1.0.0 LANGUAGES CXX)

# 设置C++标准
set(CMAKE_CXX_STANDARD 17)

set(EASYFRAMWRORKNATIVE_EXEC_PATH ${CMAKE_BINARY_DIR}/bin)
set(EASYFRAMWRORKNATIVE_LIB_PATH ${CMAKE_BINARY_DIR}/lib)

# 设置输出目录
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(EXECUTABLE_OUTPUT_PATH ${MILLION_BUILD_DIR}/bin)

add_subdirectory(third_party)
add_subdirectory(EasyFramework.Native)
add_subdirectory(example)
