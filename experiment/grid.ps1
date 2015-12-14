Param(
    [string]$dir,
    [string]$params="",
    [string]$ins,
    [switch]$fcv
);

Write-Host "Performing grid search on $ins at '$dir'...";
[System.IO.Directory]::CreateDirectory("result\$dir\Grid");

$svm_train = "svm-train.exe";
if ($fcv) {
    $svm_train = "svm-train-fcv.exe";
}

Invoke-Expression "python libsvm\tools\grid.py -svmtrain libsvm\$svm_train -gnuplot null -out result\$dir\Grid\$ins.grid -b 1 -s 0 $params result\$dir\Features\$ins.prb";