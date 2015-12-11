Param(
    [string]$dir,
    [string]$ins="",
    [string]$gen="",
    [string]$fil="",
    [string]$arc="x64"
);

Write-Host "Extracting features to '$dir'...";

if ($ins -ne ""){
    $ins = "-i " + $ins;
}
if ($gen -ne ""){
    $gen = "-g " + $gen;
}
if ($fil -ne ""){
    $fil = "-u " + $fil;
}

Invoke-Expression "..\projects\emr-coreference-resolution\Output\Debug\$arc\feat-extract.exe -d ..\dataset\i2b2_Train -m Train -o result\$dir\Features $ins $gen $fil";