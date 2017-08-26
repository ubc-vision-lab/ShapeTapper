#pragma strict

private var female : boolean = true;
private var right : boolean = true;

private var scale : float;
var stringToEdit : String;
public var Skin : GUIStyle;
public var demoSkin : GUIStyle;
public var sexSkin : GUIStyle;
public var handSkin : GUIStyle;
public var ageSkin : GUIStyle;
public var submitSkin : GUIStyle;

var m : Texture;
var f : Texture;
var r : Texture;
var l : Texture;
var submit : Texture;

function Start () {
	scale = Screen.height/6;
	Skin.fontSize = Screen.width/12;
	demoSkin.fontSize = Screen.width/30;
	ageSkin.fontSize = Screen.width/12;
	sexSkin.normal.background = f;
	handSkin.normal.background = r;
	
}

function Update () {

}
function OnGUI () {
		GUI.Label(Rect(Screen.width*0.5, Screen.height*0.2, 0, 0),'Demographics',Skin);
		
		GUI.Label(Rect(Screen.width*0.25, Screen.height*0.5-scale/6, 0, 0),'Gender',demoSkin);
		if (GUI.Button(Rect(Screen.width*0.25-scale*1.75/2, Screen.height*0.5, scale*1.75, scale),'',sexSkin)){
			female = !female;
			if(female){
				sexSkin.normal.background = f;
			}
			else{
				sexSkin.normal.background = m;
			}
			
		}
		
		GUI.Label(Rect(Screen.width*0.5, Screen.height*0.5-scale/6, 0, 0),'Handedness',demoSkin);
		if (GUI.Button(Rect(Screen.width*0.5-scale*1.75/2, Screen.height*0.5, scale*1.75, scale),'',handSkin)){
			right = !right;
			if(right){
				handSkin.normal.background = r;
			}
			else{
				handSkin.normal.background = l;
			}
			
		}
		
		if (GUI.Button(Rect(Screen.width*0.5-scale*1.75*0.6/2, Screen.height*0.85-scale*0.6/2, scale*0.6*1.75, scale*0.6),'',submitSkin)){
			var result:String = "";
			var chars:String = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
			for (var i = 0; i < 4; i++){
				result += chars[Random.Range(0,chars.Length)];
			}
			PlayerPrefs.SetString("UserID",result);
			if(!System.IO.Directory.Exists(Application.persistentDataPath+"/data_files/")){
				System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/data_files/");
			}
			var fileName = Application.persistentDataPath + "/data_files/"+PlayerPrefs.GetString("UserID","UserID")+ "_demographic.txt";
			
			var sr : StreamWriter = new StreamWriter(fileName);
			var str2write : String = "UserID,Gender,Handedness,Age,ScreenWidth,ScreenHeight,DPI,Config";
			sr.WriteLine(str2write);
			str2write = "";
			str2write += PlayerPrefs.GetString("UserID","UserID");
			str2write += ",";
			if(female){
				str2write += "F";
			}
			else{
				str2write += "M";
			}
			str2write += ",";
			if(right){
				str2write += "R";
			}
			else{
				str2write += "L";
			}
			str2write += ",";
			str2write += stringToEdit;
			str2write += ",";
			str2write += Screen.width;
			str2write += ",";
			str2write += Screen.height;
			str2write += ",";
			str2write += Screen.dpi;
			str2write += ",";
			str2write += PlayerPrefs.GetString("configName","config.txt");
			sr.WriteLine(str2write);
			
			sr.Close();
			
			Application.LoadLevel(2);
		}
			
	
		
		GUI.Label(Rect(Screen.width*0.75, Screen.height*0.5-scale/6, 0, 0),'Age',demoSkin);
		stringToEdit = GUI.TextField(Rect(Screen.width*0.75-scale*1.75/2, Screen.height*0.5, scale*1.75, scale),stringToEdit,2,ageSkin);

	}