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
    mask = new Array();

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
}