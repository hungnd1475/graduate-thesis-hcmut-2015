<?php 
	$directory = "doc\*.txt";
	$files = glob($directory,GLOB_NOSORT);

	for($i=0; $i<count($files); $i++){
		$files[$i] = str_replace("doc\\", "", $files[$i]);
	}
	sort($files, SORT_NUMERIC);

	echo json_encode($files);
?>