@echo off

echo "开始拷贝jsons"
for %%i in (*.json) do (
    echo begin copy... %%i
    copy /y %%~nxi ..\..\hola_unity\Assets\Resources\Config\%%~nxi
    echo copy complate ... %%i
)
echo "拷贝完成"

pause