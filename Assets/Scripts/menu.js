#pragma strict
import System.IO;

private var guiOn : boolean;
public var startSkin : GUIStyle;
public var helpSkin : GUIStyle;
public var resetSkin : GUIStyle;
public var UserIDSkin : GUIStyle;

private var block : int;
private var blockMod : int;
private var resetCount : int;

private var popupFlag : boolean;


function Start () {
	startSkin.fontSize = Screen.width/17;
	UserIDSkin.fontSize = Screen.width/32;
	blockMod = 1;
	guiOn = true;
	Debug.Log(PlayerPrefs.GetInt("line", 0));
	
	block = PlayerPrefs.GetInt("block", 0);
	
	resetCount = 0;
	popupFlag = false;
	
}

function Update () {
	if(Input.GetKeyDown(KeyCode.Escape)){
		Application.Quit();
	}
}

function doWindow(){
	var win_height:float;
	win_height = Screen.height*0.25;
	var win_width:float;
	win_width = Screen.width*0.4;
	
	var centeredLabel = GUI.skin.GetStyle("Label");
	centeredLabel.alignment = TextAnchor.UpperCenter;
	centeredLabel.fontSize = Screen.width/32;
	GUI.Label(Rect(win_width*0.1,win_height*0.3,win_width*0.8,win_height*0.3), "Reset the program?",centeredLabel);
	
	GUI.skin.button.fontSize = Screen.width/35;
	if(GUI.Button (Rect(win_width*0.125,win_height*0.65,win_width*0.3,win_height*0.3), "YES!")){
		guiOn = false;
		PlayerPrefs.SetString("UserID","");
		PlayerPrefs.SetInt("line",0);
		PlayerPrefs.SetInt("startNum",0);
		PlayerPrefs.SetInt("numCorrect",0);
		PlayerPrefs.SetInt("lastBlockLine", 0);
		PlayerPrefs.SetInt("block",0);
		Application.LoadLevel(0);
	}
	
	if(GUI.Button (Rect(win_width*0.575,win_height*0.65,win_width*0.3,win_height*0.3), "Cancel")){
		popupFlag = false;
	}
}


function OnGUI(){

	var button_height:float;
	button_height = Screen.height*0.15;
	var button_width:float;
	button_width = button_height*1.6;

	
	if(popupFlag){
		GUI.Window(0,Rect(Screen.width*0.3,Screen.height*0.25,Screen.width*0.4,Screen.height*0.25),doWindow,"");
	}

	if(guiOn){
		GUI.Label(Rect(Screen.width-Screen.height*.15,Screen.height*.85,0,0),"User ID: "+PlayerPrefs.GetString("UserID","UserID"),UserIDSkin);
		GUI.Label(Rect(Screen.width-Screen.height*.15,Screen.height*.85+Screen.width/30,0,0),"Blocks Done: "+block*blockMod,UserIDSkin);
		
		
		GUI.Label(Rect(Screen.width*.5,Screen.height*.25,0,0),"Shape Tapper 9000",startSkin);
		if(block >= 0){
		
			if(GUI.Button (Rect(0,Screen.height-button_height,button_width,button_height), "",UserIDSkin)){
				resetCount += 1;
			
				if(resetCount > 9){
					resetCount = 0;
					popupFlag = true;
				}
			}
				
			if(GUI.Button (Rect(Screen.width*.5-button_width/2,Screen.height*.5-button_height/2,button_width,button_height), "",startSkin) && !popupFlag){
				guiOn = false;
				PlayerPrefs.SetString("bad","");
				PlayerPrefs.SetString("Data","");
				PlayerPrefs.SetInt("badflag",0);
				PlayerPrefs.SetInt("startNum",0);
				PlayerPrefs.SetInt("endNum",0);
				PlayerPrefs.SetInt("numCorrect",0);
				PlayerPrefs.SetInt("line", PlayerPrefs.GetInt("lastBlockLine",0));
				PlayerPrefs.SetInt("feedbackLine", PlayerPrefs.GetInt("lastBlockLine",0));
				Application.LoadLevel(5);
			}
			if(GUI.Button (Rect(Screen.width*.5-button_width/2,Screen.height*.5-button_height/2+button_height*1.12,button_width,button_height), "",helpSkin) && !popupFlag){
				guiOn = false;
				Application.LoadLevel(3);
			}
		}
		else{
			blockMod = -1;
			GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Thanks for doing our experiment!",startSkin);
			if(GUI.Button (Rect(0,Screen.height-button_height,button_width,button_height), "",resetSkin)){
				guiOn = false;
				PlayerPrefs.SetString("UserID","");
				PlayerPrefs.SetInt("line",0);
				PlayerPrefs.SetInt("lastBlockLine", 0);
				PlayerPrefs.SetInt("block",0);
				Application.LoadLevel(0);
			}
		
		}
	}
}