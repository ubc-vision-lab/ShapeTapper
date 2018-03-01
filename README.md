# ShapeSpider experiment

## Setup:

The setup instructions pertain specifically to the Dell lab computer, which already has everything.
The code all works with what is listed below. Any earlier or later versions of software you use is untested, unfortunately. Your mileage may vary.

You need the following:
- MATLAB 2015b (32-bit)
  - If you open the start menu, you should see MATLAB 2015b (32-bit) in the pinned applications
  - The MATLAB script is `C:\Users\UBC\Documents\MATLAB\Rob_Test\network_listening_test.m`
  - Run the script
- PsychToolBox 3.11 (32-bit)
- NDI Optotrack (version TBD)
- ShapeSpider program
  - On the Desktop, there is a ShapeSpider program
  - Run (as of 02/26) ShapeSpider_RC4.exe

Make sure the following equipment is set up and started:

- Eyelink
  - Power to Eyelink
  - Right/Left cables plugged in, data cable plugged in
  - Eyelink Ethernet cable plugged into Dell machine
  - Eyelink attached to desk securely
  - (For hygene purposes): Tissue for the chin rest
  - Start the Eyelink Computer
- Optotrack
  - Power button on the base module (same desk as the Dell)
  - Power button on the camera module -- Ceiling (Please use a ladder, be careful!)
  - Optotrack infrared emitters on the top of the monitor stand, and one for the finger of the subject (We will try to label the finger ones)


## Running experiment:

1. If you have configuration file you want to use, place it in `C:\Users\UBC\AppData\LocalLow\EnnsLab\ShapeSpider`
1. Start the MATLAB script first -- `C:\Users\UBC\Documents\MATLAB\Rob_Test\network_listening_test.m`
1. Start ShapeSpider `C:\Users\UBC\Desktop\ShapeSpider\ShapeSpider_RC4.exe`
1. MATLAB will be ask for the User ID.
    - Unity will attempt to send a userID once it connects. However, you will likely need to reset the ShapeSpider program so that it uses your config file(bottom left of ShapeSpider menu, click 10 times). Please input the UserID listed on the bottom right of the screen into the MATLAB prompt.
1. MATLAB will ask the directory where you want to store the data. MATLAB will generate a directory with the UserID as the name once you pick.
    - If you want to run the same subject again, please just choose the same directory you initially picked, or else it will create a second directory inside (e.g. `Documents\ABCD\ABCD` rather than `Documents\ABCD`, which is what you'd want)
1. Calibrate Eyelink
   1. The screen will give you the fixation + 1-9 + shapes screen. Use that time to make sure Eyelink is working and the sensitivity and focus are all set up. Press `SPACE` once that's done.
   1. On the calibration/validation screen, either use the Eyelink Computer's mouse or the keyboard attached to the Dell to control the calibration process.
   1. Once validation is complete, press `Esc` twice to get to the fixation + 1-9 + shapes screen again. This time, a cursor will approximately follow the eye movements of the participant. Check to make sure all positions (1-9, fixation) are okay. Tell the participant to look at the fixation, then press space bar. Please make sure the participant looks at the fixation as that's how we verify that they are looking at the fixation during the experiment.
   
1. Wait for the Optotrack to initialize -- it will beep a few times, and Unity will say when Optotrack is ready.
1. Switch focus back to ShapeSpider (click on taskbar, `alt+tab`, etc.). Run the program

Some notes:
- The calibration can be re-done at any time by pressing the `F3` key.
- The fixation check is necessary for the dual task, but maybe unnecessary for the single task. In order to run the single task, you may change the `fixation_threshold` in `network_listening_test.m` to some large value (sorry!)
- The Optotrack initialization takes some time. To save time when running multiple participants sequentially, the `iniOpto` variable can be set to `1`.
- Both of the above variables are defined at the top of the `network_listening_test.m` file.

## Takedown

- ShapeSpider can be quit by pressing `Esc` from the menu.
- MATLAB should automatically quit and save the Eyelink data (the Optotrack data is saved each trial).
- Turn off Eyelink Computer (press the power button on the computer), unplug power for the Eyelink
- Turn off Optotrack

## Notes about data

- It seems there is a slight blindspot for the Optotrack that needs some investigation. We're currently uncertain why it's happening (needs detective work!)
- Eyelink data is likely not useful for the dual task, as it should simply show that the subject is looking at the fixation. The eye might dart momentarily towards the right but anything past the threshold should result in a bad trial, which would be saved in Unity.
