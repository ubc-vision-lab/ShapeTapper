#pragma strict
var scrollPosition : Vector2 = Vector2.zero;
public var backSkin : GUIStyle;
public var wordsSkin : GUIStyle;

function Start () {
	backSkin.fontSize = Screen.width/17;
	wordsSkin.fontSize = Screen.width/32;
}

function Update () {

}
function OnGUI(){

	var button_height:float;
	button_height = Screen.height*0.15;
	var button_width:float;
	button_width = button_height*1.58772;
	
	GUI.Label(Rect(Screen.width*.5,Screen.height*.15,0,0),"Instructions",backSkin);
	// An absolute-positioned example: We make a scrollview that has a really large client
	// rect and put it in a small rect on the screen.
	GUI.Box(Rect(Screen.width*0.05,Screen.height*.175,Screen.width*0.9,Screen.height*0.6),'Tap the "Start" button to start a block. Each block contains a number of trials and will take a few minutes to complete. During each trial, a shape will be presented. Please tap the shape as soon as possible to progress to the next trial.',wordsSkin);
		
	if(GUI.Button (Rect(Screen.width*.5-button_width/2,Screen.height*.5-button_height/2+button_height*2*1.12,button_width,button_height), "",backSkin)){
		Application.LoadLevel(2);
	}
}