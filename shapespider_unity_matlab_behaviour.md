# ShapeTapper Unity behaviour



## Unity behaviour

The below list of unity behaviours are listed according to Unity Scenes. Check commit logs for last update. Each has an associated script, listed in brackets.

### Start

Associated script: `menu.js`

### Check

Associated script: `check.js`

### Trials

Associated script: `trial.js`

Runs trials. Responsible to scale files, does some simpler checks to see if files exist and sends to the appropriate scene for processing.
Sometimes, the trial completes and instead of loading another trial, it reloads the scene after advancing the trial number.

### Demo

Associated script: `demo.js`

### Dot

### Help

### Trying 

### End


## Proposed behaviour, spider mode

### Trial (New)

State-based behaviour

Preliminary behaviour is more basic and does not look at the responses from Unity, though we are currently receiving responses from Unity.

Start: import configuration data if not already imported. Set up fixation cross and home object at location

Prepare for trial:

- Load one row with event information
- Load relevant assets and set in proper positions and scales
- Display fixation cross and home key
- Generate a random stream of characters, and 
- Wait for finger to be on home key.

Start trial: 



Unity and Matlab ack each other

Touch on home -> Tell MATLAB to listen to the eye
If at any point, eye deviates from fixation, MATLAB says it was bad, put trial back in pot
- or abort the trial, try again (tell participant!)

Letter is presented, 100/300 ms later, the image is presented
on finger lift, move 