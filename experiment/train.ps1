Param(
    [string]$dir,
    [string]$ins,
    [string]$params = "",
    [string]$arc="x64"
);

Write-Host "Training $ins features at $dir...";

Invoke-Expression "..\projects\emr-coreference-resolution\Output\Debug\$arc\feat-train.exe -d result\$dir\Features\$ins.prb -i $ins -o result\$dir\Models -l 1 $params";