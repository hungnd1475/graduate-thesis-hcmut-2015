@echo off
echo Resolving (%1)...
if not exist "result\%1\Resolve" mkdir "result\%1\Resolve"
..\projects\emr-coreference-resolution\Output\Debug\%3\emr-resolve.exe -d ..\dataset\i2b2_Test -m result\%1\Models -o result\%1\Resolve -s %2