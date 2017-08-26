#pragma strict
private var block : int;
private var instance : GameObject;


private var line : int;
private var time : int;
private var fileLines : Array;
private var specs : String[];
private var mySpec : String;
private var screenHeight : float;
private var screenWidth : float;
private var mask : Array;
private var mask1 : GameObject;
private var mask2 : GameObject;
private var mask3 : GameObject;
private var mask4 : GameObject;
private var mask5 : GameObject;
private var event1 : GameObject;
private var event2 : GameObject;
private var event3 : GameObject;
private var event : int;
private var frame : int;
private var clickable : boolean;

private var imgOn : int;
private var dynamicIndex : int;

public var startSkin : GUIStyle;
private var count : int;

function Start () {
	count = 0;
	startSkin.fontSize = Screen.width/30;
	
	var bad = [1,2,3,4,5,6,7];
	print(bad[3]);
	print(Array(bad[:3]+bad[3+1:]));
	
//	var test = [1,2,3,4,5];
//	test = test[2:];
//	for(var i=0;i<test.length;i++){
//		print(test[i]);
//	}
//	event = 1;
//	frame = 0;
//	time = 0;
//	PlayerPrefs.SetInt("line", 0);
//	clickable = false;
//	line = PlayerPrefs.GetInt("line", 0);
//	print("line="+line);
//	PlayerPrefs.SetInt("line", line+1);
//	fileLines = new Array();
//	ReadFile();
//	print(fileLines.length);
//	mySpec = fileLines[line];
//	specs = mySpec.Split(","[0]);
//	block = PlayerPrefs.GetInt("block", 0);
//	
//	MakeImages();
//	
//
//	
//	
//	
//	
//	
//	
//	
	mask = new Array();	
//	
	var test11:GameObject = Instantiate (Resources.Load ("Prefabs/shape1"));
	test11.GetComponent.<Renderer>().enabled = false;
	mask.Push(test11);
	var test12:GameObject = Instantiate (Resources.Load ("Prefabs/shape2"));
	test12.GetComponent.<Renderer>().enabled = false;
	mask.Push(test12);
	var test13:GameObject = Instantiate (Resources.Load ("Prefabs/shape3"));
	test13.GetComponent.<Renderer>().enabled = false;
	mask.Push(test13);
	StartCoroutine("masking");
	
	
	
	
	
	
	
	//GameObject(mask[0]).GetComponent.<Renderer>().enabled = true;
//	var cam = Camera.main;
//	var height = 2f*cam.orthographicSize;
//	var width = height * cam.aspect;
	//instance.transform.localScale.x /= 2;
	//instance.transform.localScale.y /= 2;
	//print(instance.renderer.bounds.size.y);
	//instance.transform.position= Vector3(3, 0, 0);
//	print(instance.renderer.enabled);
//	instance.renderer.enabled = !(instance.renderer.enabled);
//	print(instance.renderer.enabled);
	
}

function Update () {
	count++;
	
}
function masking () {

var timeon = 50;
imgOn = 250;

			dynamicIndex = 0;
			var temp:GameObject = mask[dynamicIndex];
			temp.GetComponent.<Renderer>().enabled = true;


while(count/60 < timeon){
			if(true){
				imgOn--;
				if(imgOn < 0){
					temp = mask[dynamicIndex];
					temp.GetComponent.<Renderer>().enabled = false;
					imgOn = 250;
					dynamicIndex = ++dynamicIndex%mask.length;
					print(dynamicIndex);
					print(mask.length);
					
					
					temp = mask[dynamicIndex];
					temp.GetComponent.<Renderer>().enabled = true;
				}
			}
			yield;
		}
		count = 0;
		if(true){
			temp = mask[dynamicIndex];
			temp.GetComponent.<Renderer>().enabled = false;
		}




}


function OnGUI(){
	GUI.Label(Rect(Screen.width*.5,Screen.height*.25,0,0),""+(count/60),startSkin);
	GUI.Label(Rect(Screen.width*.5,Screen.height*.30,0,0),""+imgOn,startSkin);
	GUI.Label(Rect(Screen.width*.5,Screen.height*.35,0,0),""+dynamicIndex,startSkin);
	GUI.Label(Rect(Screen.width*.5,Screen.height*.4,0,0),""+mask.length,startSkin);

//	var hit:RaycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//
//if(hit.collider != null)
//{
//    hit.collider.gameObject.GetComponent.<Renderer>().enabled = !(instance.GetComponent.<Renderer>().enabled);
//}
}

//function OnMouseDown () {
//	//instance.renderer.enabled = !(instance.renderer.enabled);
//	print("boom");
//}

//
//function ReadFile() {
//	try{
//		// Create an instance of StreamReader to read from a file.
//		
//        //var sr = new StreamReader(Application.persistentDataPath+"/Resources/experiment.txt");
//        var sr = new StreamReader("C:\\Users\\Paul\\Desktop\\experiment.txt");
//        
//        // Read and display lines from the file until the end of the file is reached.
//        var line1 = "";
//        while (true) {
//            line1 = sr.ReadLine();
//            if (line1 == null) {break;}
//            fileLines.Push(line1);
//        }
//        sr.Close();
//
//	}
//	catch (e) {
//		//going(1);
//		print(e);
//	}
//}
//function MakeImages() {
//	mask1 = Instantiate (Resources.Load ("Prefabs/"+specs[5]));
//	ScaleMask(mask1);
//	mask2 = Instantiate (Resources.Load ("Prefabs/"+specs[6]));
//	ScaleMask(mask2);
//	mask3 = Instantiate (Resources.Load ("Prefabs/"+specs[7]));
//	ScaleMask(mask3);
//	mask4 = Instantiate (Resources.Load ("Prefabs/"+specs[8]));
//	ScaleMask(mask4);
//	mask5 = Instantiate (Resources.Load ("Prefabs/"+specs[9]));
//	ScaleMask(mask5);
//	event1 = Instantiate (Resources.Load ("Prefabs/"+specs[11]));
//	Scale(event1,int.Parse(specs[13]));
//	event2 = Instantiate (Resources.Load ("Prefabs/"+specs[20]));
//	Scale(event2,int.Parse(specs[22]));
//	event3 = Instantiate (Resources.Load ("Prefabs/"+specs[29]));
//	Scale(event3,int.Parse(specs[31]));
//	
//}
//
//function ScaleMask(image: GameObject){
//	image.GetComponent.<Renderer>().enabled = false;
//	var cam = Camera.main;
//	screenHeight = 2f*cam.orthographicSize;
//	screenWidth = screenHeight*cam.aspect;
//	
//	var x = image.GetComponent.<Renderer>().bounds.size.x;
//	var y = image.GetComponent.<Renderer>().bounds.size.y;
//	
//	var toScalex:float = x/screenWidth;
//	var toScaley:float = y/screenHeight;
//	image.transform.localScale.x /= toScalex;
//	image.transform.localScale.y /= toScaley;
//
//}
//
//function Scale(image: GameObject, safety:float){
//	image.GetComponent.<Renderer>().enabled = false;
//	var cam = Camera.main;
//	screenHeight = 2f*cam.orthographicSize;
//	screenWidth = screenHeight*cam.aspect;
//	screenHeight *= (safety/100);
//	screenWidth *= (safety/100);
//	
//	var x = image.GetComponent.<Renderer>().bounds.size.x;
//	var y = image.GetComponent.<Renderer>().bounds.size.y;
//	var diag = Mathf.Sqrt(Mathf.Pow(x,2)+Mathf.Pow(y,2));
//	var toScale:float = diag/screenHeight;
//	image.transform.localScale.x /= toScale;
//	image.transform.localScale.y /= toScale;
//	
//	diag /= toScale;
//	var margin = (screenWidth-diag)/2;
//	var shiftAmount:float = Random.Range(-1*(margin),margin);
//	image.transform.position= Vector3(shiftAmount, 0, 0);
//	
//
//}
//function going(flag : int) {
//	PlayerPrefs.SetInt("exit_flag", flag);
//	Application.LoadLevel(2);
//}