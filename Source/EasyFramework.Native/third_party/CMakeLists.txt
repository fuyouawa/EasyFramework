set(CEREAL_HEADER_DIR ${CMAKE_CURRENT_SOURCE_DIR}/cereal/include CACHE STRING "cereal header dir")
set(CEREAL_LIB cereal CACHE STRING "cereal lib")
add_library(${CEREAL_LIB} INTERFACE)
target_include_directories(${CEREAL_LIB} INTERFACE ${CEREAL_HEADER_DIR})


set(CPPCODEC_HEADER_DIR ${CMAKE_CURRENT_SOURCE_DIR}/cppcodec CACHE STRING "cppcodec header dir")
set(CPPCODEC_LIB cppcodec CACHE STRING "cppcodec lib")
add_library(${CPPCODEC_LIB} INTERFACE)
target_include_directories(${CPPCODEC_LIB} INTERFACE ${CPPCODEC_HEADER_DIR})


set(INJA_HEADER_DIR ${CMAKE_CURRENT_SOURCE_DIR}/inja/include CACHE STRING "inja header dir")
set(INJA_LIB inja CACHE STRING "inja lib")
add_library(${INJA_LIB} INTERFACE)
target_include_directories(${INJA_LIB} INTERFACE ${INJA_HEADER_DIR})
target_include_directories(${INJA_LIB} INTERFACE ${CMAKE_CURRENT_SOURCE_DIR}/inja/third_party/include)
