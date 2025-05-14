include_directories(${EASYFRAMWRORKNATIVE_HEADER_DIR})

set(EXECUTABLE_OUTPUT_PATH ${EASYFRAMWRORKNATIVE_EXEC_PATH})

# 创建示例程序可执行文件
add_executable(example main.cpp)

# 链接主库
target_link_libraries(example PRIVATE ${EASYFRAMWRORKNATIVE_LIB})
