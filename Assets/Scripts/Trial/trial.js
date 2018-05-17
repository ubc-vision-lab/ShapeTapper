#pragma strict
import System.IO;

public var startSkin: GUIStyle;
private var badflag: int;
private var badtouch: int;
private var block: int;
private var line: int;
private var time: int;
private var fileLines: Array;
private var specs: String[];
private var mySpec: String;
private var screenHeight: float;
private var screenWidth: float;


private var mask: Array;

private var mask1: GameObject;
private var mask2: GameObject;
private var mask3: GameObject;
private var mask4: GameObject;
private var mask5: GameObject;
private var event1: GameObject;
private var event2: GameObject = null;
private var event3: GameObject = null;
private var tooSlow: GameObject;

private var event: int;
private var frame: int;
private var clickable: boolean;
private var first_run: boolean;
private var tooSlow_there: boolean;

private var tooSlow_image: String;

private var msg: int;
private var dbgmsg: String;

public var red: Texture2D;
public var green: Texture2D;

private var wp: Vector3;
private var touchPos: Vector2;


private var trialno: String;
private var badno: int;
private var startRT: float;
private var endRT: float;
private var touchXY: Vector2;

private var practice: int;
private var block_percentage: int;
private var block_fb: int;

private var repeat_trial: int;

private var feedback: int;
private var respond_time: float;
private var dyn_mask1: String;
private var dyn_mask2: String;
private var dyn_mask3: String;
private var dyn_mask4: String;
private var dyn_mask5: String;
private var x_pos: float;
private var y_pos: float;
private var onset: float;
private var e1_image: String;
private var e2_image: String;
private var e3_image: String;
private var rotation1: float;
private var rotation2: float;
private var rotation3: float;
private var safety1: float;
private var safety2: float;
private var safety3: float;
private var target_flag1: int;
private var target_flag2: int;
private var target_flag3: int;

private var dyn_mask_flag1: int;
private var dyn_mask_flag2: int;
private var dyn_mask_flag3: int;
private var total1: int;
private var total2: int;
private var total3: int;
private var image_on1: int;
private var image_on2: int;
private var image_on3: int;
private var image_off1: int;
private var image_off2: int;
private var oddball_flag: int;
private var x_pos2: int;
private var y_pos2: int;
private var x_pos3: int;
private var y_pos3: int;

var main_cam: Camera;




function Start() {
    first_run = true;
    badtouch = 0;
    badno = 0;
    tooSlow_there = false;
    startRT = 0; // time when you start trial
    endRT = 0; // time when you end trial
    touchXY = Vector2(0, 0); // take your touch
    dbgmsg = '';
    badflag = PlayerPrefs.GetInt("badflag", 0); // checks to see if it's a bad trial, behaviour changes
    startSkin.fontSize = Screen.width / 17; // #trialanderror
    msg = 0; // ???
    frame = 0; // ???
    time = 0;
    clickable = false; // controls if you can tap or not - set to true when target is 1
    line = PlayerPrefs.GetInt("line", 0); // line number in config file - increments for trial number (essentially)
    fileLines = new Array();
    mask = new Array();
    ReadFile();
    if (line >= fileLines.length) { // more trials so you're done the whole experiment woooooo
        going(9); // goes to end.js? probably maybe, definitely starts process for ending game/experiment i guess - Paul 2017
    }
    mySpec = fileLines[line];
    specs = mySpec.Split("," [0]);
    var block_num = int.Parse(specs[0]); ////// DEPENDENT ON CONFIG FILE LAYOUT

    assign_specs(specs);

    main_cam = GetComponent. < Camera > ();
    main_cam.clearFlags = CameraClearFlags.SolidColor;

    if (PlayerPrefs.GetInt("startNum", 0) == 0) { // this sets the ENTIRE BLOCK
        PlayerPrefs.SetInt("startNum", int.Parse(trialno));
        PlayerPrefs.SetInt("practice", int.Parse(specs[3])); // is this trial practice? behavioural changes?
        PlayerPrefs.SetInt("block_percentage", int.Parse(specs[4])); // percentage threshold for good/bad
        PlayerPrefs.SetInt("block_fb", int.Parse(specs[5])); // block feedback (0/1) tells you good/bad (dependent on block percentage?)
    } ////// DEPENDENT ON CONFIG FILE LAYOUT

    PlayerPrefs.SetInt("endNum", Mathf.Max(int.Parse(trialno), PlayerPrefs.GetInt("endNum", 0))); // trial you ended on?
    // does something, doesn't really matter(?)

    practice = PlayerPrefs.GetInt("practice", 0);
    block_fb = PlayerPrefs.GetInt("block_fb", 0);

    block = PlayerPrefs.GetInt("block", 0);
    if (block_num > block + 1) {
        PlayerPrefs.SetInt("badflag", 1);
        badflag = 1;
        going(0); //??
    }

    if (badflag < 1) {
        MakeImages();



        yield WaitForSeconds(onset);
        PlayerPrefs.SetInt("line", line + 1);
        StartCoroutine("Trial");
    }
}

function Update() {
    frame++;
    if (clickable) {
        if (Input.touchCount >= 2) {
            // print(Input.touchCount);
        }
        if (Input.touchCount == 1) {
            endRT = Time.time;
            touchXY = Input.GetTouch(0).position;
            // print(touchXY);
            var mouseXY = Input.mousePosition;
            //print(mouseXY);
            var hit: RaycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchXY), Vector2.zero);

            if (oddball_flag) { // just bypass and don't add to bad
                badtouch = 2;
            } else if (hit.collider != null && hit.collider.name == dbgmsg) { // dbgmsg = target name?
                badtouch = 2; // this is good?
                if (block_fb || practice) {
                    PlayerPrefs.SetInt("numCorrect", PlayerPrefs.GetInt("numCorrect", 0) + 1);
                }
            } else {
                badtouch = 1; // this probably means bad
                badno = Mathf.Max(1, badno);
                if (badno < 3) {
                    AddBad(mySpec);
                }
            }
            StartCoroutine("AddData");

        }
    }
}


function AddBad(mySpec: String) {
    var bad = PlayerPrefs.GetString("bad", "");
    if (bad != "") {
        bad += ";";
    }
    bad += mySpec;
    PlayerPrefs.SetString("bad", bad);

}

function AddData() {
    var data = PlayerPrefs.GetString("Data", "");
    if (data != "") {
        data += ";";
    }
    data += trialno + ",";
    data += badno.ToString() + ",";
    data += (endRT - startRT).ToString() + ",";
    data += (touchXY.x).ToString() + ",";
    data += (touchXY.y).ToString();
    PlayerPrefs.SetString("Data", data);

    Application.LoadLevel(4);
}

function ReadFile() {
    try {
        // Create an instance of StreamReader to read from a file.
        var sr = new StreamReader(Application.persistentDataPath + "/" + PlayerPrefs.GetString("configName", "config.txt"));

        // Read and display lines from the file until the end of the file is reached.
        var line1 = "";
        while (true) {
            line1 = sr.ReadLine();
            if (line1 == null) {
                break;
            }
            fileLines.Push(line1);
        }
        sr.Close();

    } catch (e) {
        going(1);
    }
}

function MakeImages() {
    var assetPath = Application.persistentDataPath + "/images";
    var assets = AssetBundle.LoadFromFile(assetPath);

    if (dyn_mask_flag1) {
        var mrotation = rotation1;
        var msafety = safety1;
    } else if (dyn_mask_flag2) {
        mrotation = rotation2;
        msafety = safety2;
    } else if (dyn_mask_flag3) {
        mrotation = rotation3;
        msafety = safety3;
    }

    if (dyn_mask1 != "") {
        mask1 = Instantiate(assets.LoadAsset(dyn_mask1));
        Scale(mask1, mrotation, msafety);
        mask.Push(mask1);
        if (dyn_mask2 != "") {
            mask2 = Instantiate(assets.LoadAsset(dyn_mask2));
            Scale(mask2, mrotation, msafety);
            mask.Push(mask2);
            if (dyn_mask3 != "") {
                mask3 = Instantiate(assets.LoadAsset(dyn_mask3));
                Scale(mask3, mrotation, msafety);
                mask.Push(mask3);
                if (dyn_mask4 != "") {
                    mask4 = Instantiate(assets.LoadAsset(dyn_mask4));
                    Scale(mask4, mrotation, msafety);
                    mask.Push(mask4);
                    if (dyn_mask5 != "") {
                        mask5 = Instantiate(assets.LoadAsset(dyn_mask5));
                        Scale(mask5, mrotation, msafety);
                        mask.Push(mask5);
                    }
                }
            }
        }
    }
    if (e1_image != "") {
        event1 = Instantiate(assets.LoadAsset(e1_image));
        if (oddball_flag) {
            Rescale(event1, rotation1, safety1, x_pos, y_pos);
        } else {
            Scale(event1, rotation1, safety1);
        }
    }
    if (e2_image != "") {
        event2 = Instantiate(assets.LoadAsset(e2_image));
        if (oddball_flag) {
            Rescale(event2, rotation2, safety2, x_pos2, y_pos2);
        } else {
            Scale(event2, rotation2, safety2);
        }
    }
    if (e3_image != "") {
        event3 = Instantiate(assets.LoadAsset(e3_image));
        if (oddball_flag) {
            Rescale(event3, rotation3, safety3, x_pos3, y_pos3);
        } else {
            Scale(event3, rotation3, safety3);
        }
    }

    if (tooSlow_image != "") {
        tooSlow_there = true;
        tooSlow = Instantiate(assets.LoadAsset(tooSlow_image));

        ScaleMask(tooSlow);
    }

    assets.Unload(false);
}

function ScaleMask(image: GameObject) { // non-letterboxing cropping
    image.GetComponent. < Renderer > ().enabled = false;
    var cam = Camera.main;
    screenHeight = 2f * cam.orthographicSize;
    screenWidth = screenHeight * cam.aspect;

    var x = image.GetComponent. < Renderer > ().bounds.size.x;
    var y = image.GetComponent. < Renderer > ().bounds.size.y;

    var toScalex: float = x / screenWidth;
    var toScaley: float = y / screenHeight;

    var toScale: float = Mathf.Max(toScalex, toScaley);
    image.transform.localScale.x /= toScale;
    image.transform.localScale.y /= toScale;



}

function Scale(image: GameObject, rotation: float, safety: float) {
    image.GetComponent. < Renderer > ().enabled = false;
    var cam = Camera.main;
    screenHeight = 2f * cam.orthographicSize; // half screen height times two in imaginary units (don't care about units)
    screenWidth = screenHeight * cam.aspect;
    screenHeight *= (safety / 100); // image can never be safety% of height
    screenWidth *= (safety / 100); // " width
    // diagonal of rectangle cannot be larger than the above, everything else scaled accordingly
    // i.e. measure diagonal of image, compare against height (not width) and scale dimensions

    var x = image.GetComponent. < Renderer > ().bounds.size.x;
    var y = image.GetComponent. < Renderer > ().bounds.size.y;
    var diag = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    var toScale: float = diag / screenHeight; // get ratio of diagonal to screen height
    image.transform.localScale.x /= toScale; // normalize so that the image fits that size.
    image.transform.localScale.y /= toScale; // 

    diag /= toScale;

    var worldToPixels = ((Screen.height / 2.0f) / Camera.main.orthographicSize);
    var border = Screen.dpi / worldToPixels; //3/8th of an inch #codeishard


    screenHeight = 2f * cam.orthographicSize; // shift based on x&y from camera
    screenWidth = screenHeight * cam.aspect; // 
    var margin = (screenWidth - diag - border); // x
    var toShift = x_pos / 100 * margin; // position in config is percentage of the screen // margin has no meaning
    var shiftAmount: float = toShift - (margin / 2); // final number to shift in unity

    margin = (screenHeight - diag - border);
    toShift = y_pos / 100 * margin;
    var shiftAmount2: float = toShift - (margin / 2); // y
    image.transform.position = Vector3(shiftAmount, shiftAmount2, 0);

    image.transform.eulerAngles = Vector3(0, 0, rotation);
}

// shift position by a certain amount for a game object.
// this is basically the same as Scale, except it takes x position and y position
// todo: deprecate Scale, replace usages with Rescale?
function Rescale(image: GameObject, rotation: float, safety: float, x_pos_new: float, y_pos_new: float) {
    image.GetComponent. < Renderer > ().enabled = false;
    var cam = Camera.main;
    screenHeight = 2f * cam.orthographicSize; // half screen height times two in imaginary units (don't care about units)
    screenWidth = screenHeight * cam.aspect;
    screenHeight *= (safety / 100); // image can never be safety% of height
    screenWidth *= (safety / 100); // " width
    // diagonal of rectangle cannot be larger than the above, everything else scaled accordingly
    // i.e. measure diagonal of image, compare against height (not width) and scale dimensions

    var x = image.GetComponent. < Renderer > ().bounds.size.x;
    var y = image.GetComponent. < Renderer > ().bounds.size.y;
    //print("X: " + x + ", Y: " + y);
    var diag = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    var toScale: float = diag / screenHeight; // get ratio of diagonal to screen height
    image.transform.localScale.x /= toScale; // normalize so that the image fits that size.
    image.transform.localScale.y /= toScale; // 
    //print("Transformed X: " + image.transform.localScale.x + ", Y: " + image.transform.localScale.y);

    diag /= toScale;
    //print("Diagonal: " + diag);
    var worldToPixels = ((Screen.height / 2.0f) / Camera.main.orthographicSize);
    //print("Screen height: " + Screen.height);
    //var border = Screen.dpi*0.375/worldToPixels; //3/8th of an inch #codeishard
    var border = 0;


    screenHeight = 2f * cam.orthographicSize; // shift based on x&y from camera
    screenWidth = screenHeight * cam.aspect; // 

    var margin = (screenWidth - diag - border); // x
    //print('Margin: ' + margin);
    var toShift = x_pos_new / 100 * margin; // position in config is percentage of the screen // margin has no meaning
    var shiftAmount: float = toShift - (margin / 2); // final number to shift in unity
    //print("x: " + x_pos_new);
    //print("y: " + y_pos_new);
    margin = (screenHeight - diag - border);
    toShift = y_pos_new / 100 * margin;
    var shiftAmount2: float = toShift - (margin / 2); // y
    image.transform.position = Vector3(shiftAmount, shiftAmount2, 0);

    image.transform.eulerAngles = Vector3(0, 0, rotation);
}

function Trial() {
    if (oddball_flag) { // oddball mode
        main_cam.backgroundColor = Color.black;
        for (var i = 0; i < repeat_trial; i++) { // because repeat_trial means something?
            //print(trialno);
            frame = 0; //reset the frame counter
            startRT = Time.time; // start the timer!

            // make the assumption that all events exist and don't need to be checked
            clickable = true;
            dbgmsg = event1.name; // maybe add which image was touched after?
            //print("Event 1: " + event1.name);
            //print("Event 2: " + event2.name);
            //print("Event 3: " + event3.name);
            event1.GetComponent. < Renderer > ().enabled = true;
            if (event2 != null) {
                event2.GetComponent. < Renderer > ().enabled = true;
            }
            if (event3 != null) {
                event3.GetComponent. < Renderer > ().enabled = true;
            }

            StartCoroutine("WaitandGo");
            yield WaitForSeconds(total1);

            clickable = false;
            event1.GetComponent. < Renderer > ().enabled = false;
            if (event2 != null) {
                event2.GetComponent. < Renderer > ().enabled = false;
            }
            if (event3 != null) {
                event3.GetComponent. < Renderer > ().enabled = false;
            }
            frame = 0;
        }
        main_cam.backgroundColor = Color.white;
    } else {
        for (var round = 0; round < repeat_trial; round++) { // don't fuck up experimenters.
            //print('inloop');
            event = 1;
            frame = 0;
            var isMask = 0;
            var isTarget = 0;
            if (event == 1 && (e1_image != '' || dyn_mask_flag1 == 1 && dyn_mask1 != '')) { // first event or dynamic mask
                var timeon = total1; // set the time
                var timeoff = image_off1;
                isTarget = target_flag1;
                isMask = dyn_mask_flag1;
                if (isMask) { // it's a mask, let the the loop for the mask begin
                    var imgOn = image_on1;
                    var dynamicIndex = 0;
                    var temp: GameObject = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = true;
                } else if (first_run && isTarget) { // if it's the first run of the target, start the timer
                    startRT = Time.time;
                    event1.GetComponent. < Renderer > ().enabled = true;
                    clickable = true;
                    dbgmsg = event1.name;
                    StartCoroutine("WaitandGo");
                } else { //
                    event1.GetComponent. < Renderer > ().enabled = true;
                }
                while (frame < timeon) {
                    if (isMask) {
                        imgOn--;
                        if (imgOn < 0) { // cycle to all the images
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = false;
                            imgOn = image_on1;
                            dynamicIndex = ++dynamicIndex % mask.length; // this lets it repeat
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = true;
                        }
                    }
                    yield;
                }
                frame = 0;
                if (isMask) {
                    temp = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = false;
                }
                event1.GetComponent. < Renderer > ().enabled = false;
                while (frame < timeoff) {
                    yield;
                }
                event = 2;
                frame = 0;
            }

            if (event == 2 && (e2_image != '' || dyn_mask_flag2 == 1 && dyn_mask2 != '')) {
                timeon = total2;
                timeoff = image_off2;
                isTarget = target_flag2;
                isMask = dyn_mask_flag2;
                if (isMask) {
                    imgOn = image_on2;
                    dynamicIndex = 0;
                    temp = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = true;
                } else if (first_run && isTarget) {
                    startRT = Time.time;
                    event2.GetComponent. < Renderer > ().enabled = true;
                    clickable = true;
                    dbgmsg = event2.name;
                    StartCoroutine("WaitandGo");
                } else {
                    event2.GetComponent. < Renderer > ().enabled = true;
                }
                while (frame < timeon) {
                    if (isMask) {
                        imgOn--;
                        if (imgOn < 0) {
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = false;
                            imgOn = image_on2;
                            dynamicIndex = ++dynamicIndex % mask.length;
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = true;
                        }
                    }
                    yield;
                }
                if (isMask) {
                    temp = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = false;
                }
                event2.GetComponent. < Renderer > ().enabled = false;
                frame = 0;
                while (frame < timeoff) {
                    yield;
                }
                event = 3;
                frame = 0;
            }

            if (event == 3 && (e3_image != '' || dyn_mask_flag3 == 1 && dyn_mask3 != '')) {
                timeon = total3;
                isTarget = target_flag3;
                isMask = dyn_mask_flag3;
                if (isMask) {
                    imgOn = image_on3;
                    dynamicIndex = 0;
                    temp = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = true;
                } else if (first_run && isTarget) {
                    startRT = Time.time;
                    event3.GetComponent. < Renderer > ().enabled = true;
                    clickable = true;
                    dbgmsg = event3.name;
                    StartCoroutine("WaitandGo");
                } else {
                    event3.GetComponent. < Renderer > ().enabled = true;
                }
                while (frame < timeon) {
                    if (isMask) {
                        imgOn--;
                        if (imgOn < 0) {
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = false;
                            imgOn = image_on3;
                            dynamicIndex = ++dynamicIndex % mask.length;
                            temp = mask[dynamicIndex];
                            temp.GetComponent. < Renderer > ().enabled = true;
                        }
                    }
                    yield;
                }
                if (isMask) {
                    temp = mask[dynamicIndex];
                    temp.GetComponent. < Renderer > ().enabled = false;
                }
                event3.GetComponent. < Renderer > ().enabled = false;
                frame = 0;
                event = 4;
            }
            first_run = false;
        }
    }

    return;
}

function WaitandGo() {
    yield WaitForSeconds(respond_time);
    clickable = false;
    msg = 1;
    yield WaitForSeconds(1);
    badno = Mathf.Max(2, badno);
    if (badno <= 2) {
        AddBad(mySpec);
    }
    StartCoroutine("AddData");
}



function OnGUI() { // "deprecated, but it works"^TM

    if (msg == 1) {
        if (tooSlow_there && tooSlow.GetComponent. < Renderer > ().enabled == false) {
            tooSlow.GetComponent. < Renderer > ().enabled = true;
        } else {
            GUI.Label(Rect(Screen.width * .5, Screen.height * .5, 0, 0), "Too slow...", startSkin);
        }
    }
    if (msg == 0 && tooSlow_there && tooSlow.GetComponent. < Renderer > ().enabled == true) {
        tooSlow.GetComponent. < Renderer > ().enabled = false;
    }
    if (feedback) { // 0 = no feedback
        var colPreviousGUIColor: Color = GUI.color;
        GUI.color = new Color(colPreviousGUIColor.r, colPreviousGUIColor.g, colPreviousGUIColor.b, 0.85);
        if (badtouch == 1) { // feedback >= 1 means shows bad touches
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), red);
        } else if (badtouch == 2 && feedback == 1) { // feedback = 1 shows good touches
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), green);
        }
        GUI.color = colPreviousGUIColor;
    }
}

function going(flag: int) {
    var bad = PlayerPrefs.GetString("bad", "");
    if (bad != "") {
        DoBad();
    } else {
        PlayerPrefs.SetInt("badflag", 0);
        PlayerPrefs.SetInt("exit_flag", flag);
        PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
        Application.LoadLevel(5);
    }
}

function DoBad() {
    PlayerPrefs.SetInt("line", line);
    var bad = PlayerPrefs.GetString("bad", "").Split(";" [0]);
    PlayerPrefs.SetString("bad", "");
    var badTrial = Random.Range(0, bad.Length);
    mySpec = bad[badTrial];
    bad = bad[: badTrial] + bad[badTrial + 1: ];
    for (var i = 0; i < bad.length; i++) {
        AddBad(bad[i]);
    }
    specs = mySpec.Split("," [0]);

    assign_specs(specs);

    MakeImages();

    yield WaitForSeconds(onset);

    StartCoroutine("Trial");
}

function assign_specs(specs: Array) { ////// DEPENDENT ON CONFIG FILE LAYOUT
    // specs[0] is block number
    trialno = specs[1];
    feedback = int.Parse(specs[2]); // 1 or 0 - when you tap, you get a green flash for good, or red if you done goof'd
    // 0 = none, 1/2/3 = ??? (check 1/2/3 in code (trial))
    // 3,4,5????????
    respond_time = float.Parse(specs[6]); // time before trial will timeout
    // (with the bad flag and the trial gets moved into a pool of bad trials
    // and then the pool of bad trials will replay randomly from pool until no more bad trials)
    // bad trials will go back if bad again...
    // basically redemption is inevitable
    tooSlow_image = specs[7]; // this is what plays when you're too slow. so... find something funny or depressing (or Rob's specs)

    dyn_mask1 = specs[8];
    dyn_mask2 = specs[9];
    dyn_mask3 = specs[10];
    dyn_mask4 = specs[11];
    dyn_mask5 = specs[12];

    repeat_trial = int.Parse(specs[13]); // how many times the trial is played

    x_pos = float.Parse(specs[14]);
    y_pos = float.Parse(specs[15]);

    onset = float.Parse(specs[16]); // psychology jargon blank time before presentation of ~stym pack~ stimulus

    e1_image = specs[17]; // do not add file extension
    e2_image = specs[25];
    e3_image = specs[33];

    rotation1 = float.Parse(specs[18]); // degree of rotation (CW/CCW find out pls)
    rotation2 = float.Parse(specs[26]);
    rotation3 = float.Parse(specs[34]);

    safety1 = float.Parse(specs[19]); // safety margin
    safety2 = float.Parse(specs[27]); // fuck how to explain
    safety3 = float.Parse(specs[35]); // safety margin for images (no tapping on edge of screen)

    target_flag1 = int.Parse(specs[20]); // 0 or 1
    target_flag2 = int.Parse(specs[28]); // only one target flag
    target_flag3 = int.Parse(specs[36]); // there's only one target
    // when you tap, you compare the name of the tapped image with the name of the event with target_flag as 1

    dyn_mask_flag1 = int.Parse(specs[21]); // probably 0 or 1
    dyn_mask_flag2 = int.Parse(specs[29]); // sets if event is dynamic mask
    dyn_mask_flag3 = int.Parse(specs[37]); // there is only one dynamic mask per trial and it can only be presented once

    total1 = int.Parse(specs[22]); // controls timings for each event in FRAMES
    total2 = int.Parse(specs[30]); // total on time for the event
    total3 = int.Parse(specs[38]);

    image_on1 = int.Parse(specs[23]); // only comes up on the dynamic mask, ideally smaller than total1, evenly divides into total1
    image_on2 = int.Parse(specs[31]); // summation should be <= total1, if greater will be cut
    image_on3 = int.Parse(specs[39]);

    image_off1 = int.Parse(specs[24]); // time between event n and n+1
    image_off2 = int.Parse(specs[32]); // no image_off3 because end of trial

    oddball_flag = int.Parse(specs[40]); // oddball comparison mode
    x_pos2 = float.Parse(specs[41]);
    y_pos2 = float.Parse(specs[42]);
    x_pos3 = float.Parse(specs[43]);
    y_pos3 = float.Parse(specs[44]);
}

function OnApplicationPause(pauseStatus: boolean) {
    if (badno < 3) {
        AddBad(mySpec);
        badno = 3;
    }
}
