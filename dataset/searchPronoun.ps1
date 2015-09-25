param($term)
Get-ChildItem -recurse | Select-String -pattern c=`"$term`" | Select-String -pattern t=`"pronoun`" | group path | select name