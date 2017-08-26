#pragma strict
public var centerStyle : GUIStyle;
private var isIn : boolean;
private var wp : Vector3;
private var touchPos : Vector2;
private var touchSpot : Vector2;
public var dot : Texture2D;

function Start () {

	touchSpot = new Vector2(0, 0);
}

function Update () {
	if (Input.touchCount == 1){
         wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
         touchPos = new Vector2(wp.x, wp.y);
         touchSpot = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
     }







	if(Input.GetKeyDown(KeyCode.Escape)){
		Application.Quit();		
	}
}

function OnGUI(){

	if(touchSpot.x > 0){
		GUI.DrawTexture(Rect(touchSpot.x-4, Screen.height-touchSpot.y-4 ,8,8), dot, ScaleMode.ScaleToFit, true, 0);
		
		
	}
	
}