using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TryingStringStream : Observer {

    // Configuration variables
    private string assetPath;
    private string configDataPath;
    private TextAsset experimentConfigFile;
    private List<TrialConfig> experimentConfig;
    private List<StimulusEvent> trialEvents;
    private GameObject fingerStart;

    /**
     * Trial configuration. Holds data pertaining to one trial in an experiment
     * 
     **/
    public class TrialConfig
    {
        private List<string> trialConfig;

        public TrialConfig(string config)
        {
            trialConfig = new List<string>(config.Split(','));
        }
    }
    
    // tracking current trial information
    private int _absolute_trial_number;
    private string previous_response = "";

    public int Absolute_trial_number
    {
        get
        {
            return _absolute_trial_number;
        }

        set
        {
            Assert.IsTrue(_absolute_trial_number >= 0,"Absolute trial number negative!");
            Assert.IsTrue(_absolute_trial_number < experimentConfig.Count , "Absolute trial number is larger than number of trials!");
            _absolute_trial_number = value;
        }
    }

    public string ConfigDataPath
    {
        get
        {
            return configDataPath;
        }

        set
        {
            if(!File.Exists(value))
            {
                PlayerPrefs.SetInt("flag", 1); // this is the config file error flag
                SceneManager.LoadScene("end");
            }
            configDataPath = value;
        }
    }

    #region UnityEvents
    // Use this for initialization
    void Start() {

        this.assetPath = Application.persistentDataPath + "/images";
        this.ConfigDataPath = Application.persistentDataPath + "/" + PlayerPrefs.GetString("config", "config.txt");
        if(!File.Exists(this.ConfigDataPath))
        {

        }
        this.Absolute_trial_number = PlayerPrefs.GetInt("line", 0);
        // fetch current experiment state
        // block number, subjectID
        // 
        

        // Generate a random string of letters and numbers
        int streamSize = 20 + Random.Range(0, 6);
        int repeatDistance = 2;
        string[] charStream = this.MakeCharStream(streamSize, repeatDistance);

        StartCoroutine(StreamChars(charStream, 0.03f, 0.10f));
    }

    // Update is called once per frame
    void Update () {
        string response = MATLABclient.mlClient.GetResponse();
        if(response.Equals("Fixation"))
        {
            // all clear;
        }
        else if(response.Equals("NotFixation"))
        {
            // end all coroutines
            // StopCoroutine()
        }
        // keep track of trial running state
        // create fixation and home button
        // wait until trial finished (yield?)
        // check for collisions
    }

    private void FixedUpdate()
    {
        // save touch position
        if(Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            // save the mouse position
        }
    }
    #endregion UnityEvents

    #region characterStream
    // Flashing Text assets
    public Text charStreamText;
    private const string letters = "abcdefghijklmnopqrstuvwxyz";
    private string targetLetter = " ";

    IEnumerator StreamChars(string[] charStream, float displayDuration, float displayInterval)
    {
        foreach(string character in charStream)
        {
            // display the letter at that position
            this.charStreamText.text = character;
            float displayStart = Time.time;
            yield return new WaitForSeconds(displayDuration);
            // turn off the letter
            this.charStreamText.text = "";
            float actualDuration = Time.time - displayStart;
            yield return new WaitForSeconds(displayInterval - actualDuration); // use actualDuration to maintain interval length as much as possible
        }
        Debug.Log("Done.");
    }

    public string[] MakeCharStream(int length, int repeat_distance)
    {
        string[] generatedString = new string[length];
        string previous_chars = new string(' ',repeat_distance); // "Empty" string
        for (int i = 0; i < generatedString.Length; i++)
        {
            string tmp = Random.Range(0, 10).ToString(); // all the single digit numbers
            while (previous_chars.Contains(tmp)) // until there's no string that's the same as the first two
            {
                tmp = Random.Range(0, 10).ToString();
            }
            previous_chars.Insert(i % repeat_distance, tmp); // last two characters
            generatedString[i] = tmp;
        }
        return generatedString;
    }

    /**
     * Inserts a letter into the array of characters between 5 and the end
     * of the array. Sets the letter in this instance.
     * 
     * Parameters: charStream -- Stream of number characters to be presented
     * Output: modified charStream with Target letter inside
     * Effects: Inserts Target Letter into charStream.
     */
    public string[] InsertLetter(string[] charStream)
    {
        Assert.IsTrue(charStream.Length >= 20);
        targetLetter = letters[Random.Range(0, letters.Length)].ToString();
        int charBegin = Random.Range(5, 10);
        charStream[Random.Range(charBegin, charStream.Length)] = targetLetter;
        return charStream;
    }
    #endregion characterStream

    #region setup stuff
    public void ReadConfigFile()
    {
        experimentConfig = new List<TrialConfig>();
        StreamReader configStream = new StreamReader(this.ConfigDataPath);
        while (configStream.Peek() >= -1)
        {
            experimentConfig.Add(new TrialConfig(configStream.ReadLine()));
        }
        configStream.Close();
    }

    public void CreateStimuli()
    {
        
    }
    #endregion

    #region behaviour
    private float trial_onset_time;

    IEnumerator InitFixation()
    {
        string[] charStream = InsertLetter(MakeCharStream(Random.Range(20, 26), 2));
        fingerStart.GetComponent<Renderer>().enabled = true;
        Coroutine streamRoutine = StartCoroutine(StreamChars(charStream, 0.07f, 0.1f));
        yield return new WaitForSeconds(trial_onset_time);
        bool runTrial = Input.GetMouseButtonDown(0);
        if (runTrial)
        {
            RaycastHit2D clickedObject = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
            if(clickedObject.collider == null)
            {
                // kill the character stream
                StopCoroutine(streamRoutine);
                // Give feedback;
                // re-add trial to trial list
            }
            else if (clickedObject.collider.name == fingerStart.name)
            {
                // send begin message to MATLAB
                // start presenting stimuli
                MATLABclient.mlClient.SendOptotrackBegin();
                yield return PresentStimuli();
                MATLABclient.mlClient.SendTrialEnd();
                // ask for target letter
                yield return AskForLetter();
                Absolute_trial_number++;
            }

        }
    }


    /**
     * Runs the trial by going through all events and presenting them.
     */
    IEnumerator PresentStimuli() // should take in trial information
    {
        foreach(StimulusEvent trialEvent in trialEvents){
            yield return trialEvent.Present();
        }
    }

    IEnumerator AskForLetter()
    {
        yield return null;
    }

    public override void OnNotify()
    {
        MATLABclient.mlClient.GetResponse();
    }
    #endregion behaviour


}
