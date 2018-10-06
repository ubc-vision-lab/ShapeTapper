#pragma strict
import System;
import System.IO;

private var flag : int;
private var block : int;
public var endSkin : GUIStyle;

private var ur_score : float;
private var to_pass : float;
private var out_of : int;


function Start () {
	endSkin.fontSize = Screen.width/17;
	flag = PlayerPrefs.GetInt("exit_flag", 0);
	block = PlayerPrefs.GetInt("block", 0);
	
	out_of = PlayerPrefs.GetInt("endNum", 60);
	to_pass = PlayerPrefs.GetInt("block_percentage", 50);
	ur_score = PlayerPrefs.GetInt("numCorrect", 1000)/(out_of*1.00)*100.00;

	if((flag == 0 || flag == 9) && PlayerPrefs.GetInt("practice", 1) && ur_score < to_pass){
		PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("feedbackLine",0));
	}
	else if(flag == 0){
		PlayerPrefs.SetInt("block",block+1);
	}
	else if(flag == 9){
		PlayerPrefs.SetInt("block", -1*(block+1));
	}
	
	StartCoroutine("ShortPause");

	


}

function Update () {

}

function OnGUI(){
	//0=ok
	//1=error
	//9=done experiment
	if(flag == 0 || flag == 9){
		if(PlayerPrefs.GetInt("block_fb",0) && ur_score >= to_pass){
			//GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Finished block "+(block+1)+"\nPassing accuracy: "+PlayerPrefs.GetInt("block_percentage", 50)+"\nYou got: "+(ur_score/(out_of*1.00)*100).ToString("F0")+"%\nGood job!", endSkin);
			GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Finished block "+(block+1)+"\nPassing accuracy: "+to_pass.ToString("F1")+"%\nYou got: "+ur_score.ToString("F1")+"%\nGood job!", endSkin);
		}
		else if(PlayerPrefs.GetInt("block_fb",0) && ur_score < to_pass){
			//GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Finished block "+(block+1)+"\nPassing accuracy: "+PlayerPrefs.GetInt("block_percentage", 50)+"\nYou got: "+(ur_score/(out_of*1.00)*100).ToString("F0")+"%\nTry harder next time!", endSkin);
			GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Finished block "+(block+1)+"\nPassing accuracy: "+to_pass.ToString("F1")+"%\nYou got: "+ur_score.ToString("F1")+"%\nTry harder next time!", endSkin);
		}
		else{
			GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"Finished block "+(block+1), endSkin);
		}
		
	}
	else if(flag == 1){
		GUI.Label(Rect(Screen.width*.5,Screen.height*.5,0,0),"TXT file error!\nPlease contact Experimenter.", endSkin);
	}

}

function ShortPause(){
	if(!System.IO.Directory.Exists(Application.persistentDataPath+"/data_files/")){
			System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/data_files/");
		}
		
	var fileName = Application.persistentDataPath + "/data_files/"+PlayerPrefs.GetString("UserID","dataFile")+"_" + (block+1) + ".txt";

	var sr : StreamWriter = new StreamWriter(fileName);
	var str2write : String = "trial#\tgood_trial\ttarget\tplace\tletter_onset\tanswer\tonset\tlift\t1sttouch\t1sttchptx\t1sttchpty\tlasttouch\tlasttchptx\tlasttchpty\teventpositions";
	sr.WriteLine(str2write);
	
	var Data = PlayerPrefs.GetString("Data","").Split(";"[0]);
	PlayerPrefs.SetString("Data","");
	
	for(var i=0;i<Data.length;i++){
		str2write = Data[i];
		sr.WriteLine(str2write);
	}
	sr.Close();
	
	yield WaitForSeconds(5);
	SceneManagement.SceneManager.LoadScene(2);
}
