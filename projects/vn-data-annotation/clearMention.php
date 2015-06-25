<?php 
	if($_SERVER['REQUEST_METHOD'] == "POST"){
		$postdata = file_get_contents("php://input");
		$request = json_decode($postdata);
		if(!array_key_exists("file", $request)){
			echo "Error";
			return;
		}
		$filename = $request->file;
		unlink("html/".$filename);
		unlink("mention/".$filename);
		Echo "Success";
	}
?>