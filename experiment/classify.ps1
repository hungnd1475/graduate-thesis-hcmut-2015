Param(
    [string]$dir,
    [string]$ins,
    [switch]$fcv
);

Write-Host "Classifying $ins at '$dir'...";
[System.IO.Directory]::CreateDirectory("result\$dir\Clas");

$svm_predict = "svm-predict.exe";
if ($fcv) {
    $svm_predict = "svm-predict-fcv.exe";
}

Invoke-Expression "libsvm\$svm_predict -b 1 result\$dir\Features\$ins.prb result\$dir\Models\$ins.model result\$dir\Clas\$ins.predict";