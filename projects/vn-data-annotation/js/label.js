// var mentions = [];

////////////////////////////Get and mark selected text////////////////////////
function getSelected(element) {
	var start = 0, end = 0, text="", line="";
	var sel, range, priorRange;
	if (typeof window.getSelection != "undefined") {
		text = window.getSelection();

		var parent = text.anchorNode.parentElement;
		var line = $(parent).index()+1;

		range = window.getSelection().getRangeAt(0);
		priorRange = range.cloneRange();
		priorRange.selectNodeContents(element);
		priorRange.setEnd(range.startContainer, range.startOffset);
		start = priorRange.toString().length;
		end = start + range.toString().length - 1;
	} else {
		alert("Plz use Chrome or Firefox");
	}
	return {
		obj: text,
		line: line,
		start: start,
		end: end
	};
}

function highlight(element, selected){
	var span = document.createElement("span");
	span.className="selected";
	span.setAttribute("start", selected.start);
	span.setAttribute("end", selected.end);
	span.setAttribute("line", selected.line);
	element.surroundContents(span);
}

$("#emr-doc").on("mouseup", ".sentence",function(){
	var selected = getSelected($(this).get(0));
	if(selected.obj.toString().length > 0){
		highlight(selected.obj.getRangeAt(0), selected);
	}
})
////////////////////////////////////// End //////////////////////////////////


/////////////////////////////////Add Label///////////////////////////////////
$("#emr-doc").on("click", ".sentence .selected",function(){
	$("#dialog").data("obj",$(this)).dialog("open");
})

$("#dialog").dialog({
	resizable: false,
	draggable: false,
	autoOpen: false,
	modal: true,
	width: 200,
	buttons:{
		OK: function(){
			var label = $("input[name=label]:checked").val();
			insertMention($("#dialog").data("obj"), label);
			if(label=="clear"){
				$("#dialog").data("obj").contents().unwrap();
			} else {
				$("#dialog").data("obj").removeClass().addClass("selected").addClass(label);
			}
			$(this).dialog("close");
		},
		Cancel: function(){
			$(this).dialog("close");
		}
	}
})
////////////////////////////////////// End //////////////////////////////////


/////////////////////////////////Add Mention/////////////////////////////////
function insertMention(element, label){
	var line = element.attr("line");
	var start = element.attr("start");
	var end = element.attr("end");
	var string = element.text();
	
	var selected = 'c="'+string+'" '+line+":"+start+" "+line+":"+end;
	var mention = selected+'||t="'+label+'"';

	//Get angular scope of emrList controller
	var scope = angular.element(document.getElementById("wrap")).scope();
	scope.$apply(function(){
		var found = false;
		for(var i=0; i<scope.mentions.length; i++){
			var exist = scope.mentions[i].indexOf(selected);
			if(exist > -1){
				found = true;
				if(label!="clear"){
					scope.mentions[i] = mention;
				} else {
					scope.mentions.splice(i, 1);
				}
			}
		}
		if(found==false && label!="clear"){
			scope.mentions.push(mention);
		}
	})
}
////////////////////////////////////// End //////////////////////////////////