@echo off
echo Performing grid search on %2 (%1)...
if not exist "result\%1\Grid" mkdir "result\%1\Grid"
python libsvm\tools\grid.py -svmtrain libsvm\svm-train.exe -gnuplot null -out result\%1\Grid\%2.grid "-b 1 -s 0" %~3 result\%1\Features\%2.prb