#pragma strict
import System.IO;

private var test : GameObject;
private var guiOn : int;
public var startSkin : GUIStyle;
private var fileLines : Array;

private var txtPath : String;
private var assetPath : String;
private var error_msg : String;


var stringToEdit : String;
public var configfileSkin : GUIStyle;
public var submitSkin : GUIStyle;

private var popupFlag : boolean;


function Start () {

	if(PlayerPrefs.GetString("UserID","") != ""){
			Application.LoadLevel(2);
	}
		
	stringToEdit = "config.txt";
	configfileSkin.fontSize = Screen.width/20;
	
	
	popupFlag = false;
	

}

function doWindow(){
	var win_height:float;
	win_height = Screen.height*0.25;
	var win_width:float;
	win_width = Screen.width*0.4;
	
	var centeredLabel = GUI.skin.GetStyle("Label");
	centeredLabel.alignment = TextAnchor.UpperCenter;
	centeredLabel.fontSize = Screen.width/32;
	GUI.Label(Rect(win_width*0.1,win_height*0.3,win_width*0.8,win_height*0.6), "Confirm file name:\n"+stringToEdit,centeredLabel);
	
	GUI.skin.button.fontSize = Screen.width/35;
	
	if(GUI.Button (Rect(win_width*0.125,win_height*0.95,win_width*0.3,win_height*0.3), "OK!")){
		popupFlag = false;
		masterChecker(stringToEdit);
	}
	
	if(GUI.Button (Rect(win_width*0.575,win_height*0.95,win_width*0.3,win_height*0.3), "Cancel")){
		popupFlag = false;
	}
}

function masterChecker(configName){
	error_msg = '';
	startSkin.fontSize = Screen.width/17;
	guiOn = 0;
	fileLines = new Array();
	txtPath = Application.persistentDataPath+'/'+configName;
	assetPath = Application.persistentDataPath+"/images";
	print("Persistent datapath:" + Application.persistentDataPath);
	if(!System.IO.File.Exists(txtPath)){
		guiOn = 1;
	}
	else{
		ReadFile();
		if(!System.IO.File.Exists(assetPath)){
			guiOn = 3;
		}
		else{
			var assets = AssetBundle.LoadFromFile(assetPath);
			for(var i=0; i < fileLines.length; i++){
				var mySpec : String = fileLines[i];
				var specs = mySpec.Split(","[0]);
				if(!CheckImages(specs, assets)){
					guiOn = 4;
					break;
				}
				else{
					guiOn = 5;
				}
			}
			assets.Unload(false);
		}
	}
	if(guiOn == 5){
		PlayerPrefs.SetString("configName",stringToEdit);
		Application.LoadLevel(1);
	}
}

function CheckImages(specs:Array,assets:AssetBundle) {
	var image_numbers = [7,8,9,10,11,12,17,25,33]; //dyn_mask images 1-5 and E1-3 images             /////// DEPENDENT ON CONFIG FILE LAYOUT
	// every trial, three events
	// event = thing that pops up
	// one dynamic mask = one event (up to five things cycling through)
	// e.g. dynamic mask has 5 images each for 0.5 seconds, for 5 seconds
	// will loop back - there are two durations (only for dyamic mask) - the fields exist for all images but not all need them

	// event timing and duration up to config
	// e.g. image -> mask -> image
	for(var i=0; i < image_numbers.length; i++){
		if(specs[image_numbers[i]] != ""){
			if(!assets.Contains(specs[image_numbers[i]])){
				error_msg = specs[image_numbers[i]];
				return false;
			}
		}
	}
	return true;
}

function Update () {

}

function OnGUI(){

	if(popupFlag){
		GUI.Window(0,Rect(Screen.width*0.3,Screen.height*0.25,Screen.width*0.4,Screen.height*0.35),doWindow,"");
	}

	var scale = Screen.height/8;
	
	stringToEdit = GUI.TextField(Rect(Screen.width*0.15, Screen.height*0.8, Screen.width*0.5, scale),stringToEdit,configfileSkin);

	if (GUI.Button(Rect(Screen.width*0.7, Screen.height*0.8, scale*1.717, scale),'',submitSkin)){
		popupFlag = true;
	}

	if(guiOn == 1){
		GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Config file not found!\nPlease contact Experimenter.",startSkin);
	}
	else if(guiOn == 2){
		GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Config file error!\nPlease contact Experimenter.",startSkin);
	}
	else if(guiOn == 3){
		GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Assetbundle file not found!\nPlease contact Experimenter.",startSkin);
	}
	else if(guiOn == 4){
		GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),error_msg+" not found!\nPlease contact Experimenter.",startSkin);
	}
	
}

function ReadFile() {

	try{
		// Create an instance of StreamReader to read from a file.
        var sr = new StreamReader(txtPath);
        // Read and display lines from the file until the end of the file is reached.
        var line1 = "";
        while (true) {
            line1 = sr.ReadLine();
            if (line1 == null) {break;}
            fileLines.Push(line1);
        }
        sr.Close();

	}
	catch (e) {
		guiOn = 2;
	}
}
