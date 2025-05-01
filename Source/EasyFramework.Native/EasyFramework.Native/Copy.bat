@echo off
setlocal

rem 设置源路径（相对于bat的位置）
set SOURCE_DEBUG=..\x64\Debug\EasyFramework.Native.dll
set SOURCE_DEBUG_PDB=..\x64\Debug\EasyFramework.Native.pdb
set SOURCE_RELEASE=..\x64\Release\EasyFramework.Native.dll
set SOURCE_RELEASE_PDB=..\x64\Release\EasyFramework.Native.pdb

rem 设置目标路径
set TARGET_DEBUG=..\..\..\Assets\Plugins\EasyFramework\Assemblies
set TARGET_RELEASE=..\..\..\Assets\Plugins\EasyFramework\Assemblies

rem 根据编译模式选择复制
if /i "%1"=="Debug" (
    if not exist "%TARGET_DEBUG%" mkdir "%TARGET_DEBUG%"
    copy /y "%SOURCE_DEBUG%" "%TARGET_DEBUG%"
) else if /i "%1"=="Release" (
    if not exist "%TARGET_RELEASE%" mkdir "%TARGET_RELEASE%"
    copy /y "%SOURCE_RELEASE%" "%TARGET_RELEASE%"
) else (
    echo [Error] Specify Debug or Release in the argument.
    exit /b 1
)

echo [Info] DLL Copy Success.
endlocal
