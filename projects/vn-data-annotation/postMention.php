<?php 
	if($_SERVER['REQUEST_METHOD'] == "POST"){
		$postdata = file_get_contents("php://input");
		$request = json_decode($postdata);
		if(!array_key_exists("file", $request) || !array_key_exists("mentions", $request) || !array_key_exists("html", $request)){
			echo "Error";
			return;
		}
		$filename = $request->file;
		$mentions = $request->mentions;
		$html = $request->html;

		$file = fopen("mention/".$filename, "w+");
		for($i=0; $i<count($mentions); $i++){
			fwrite($file, $mentions[$i]."\n");
		}
		fclose($file);

		$file = fopen("html/".$filename, "w+");
		for($i=0; $i<count($html); $i++){
			fwrite($file, $html[$i]."\n");
		}
		fclose($file);

		Echo "Success";
	}
?>