@echo off
echo Classifying %2 (%1)...
if not exist "result\%1\Clas" mkdir "result\%1\Clas"
libsvm\svm-predict -b 1 result\%1\Features\%2.prb result\%1\Models\%2.model result\%1\Clas\%2.predict