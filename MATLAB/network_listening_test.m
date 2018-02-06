% Unity connection setup
UnityPort = tcpip('localhost', 56789, 'NetworkRole', 'Server');
set(UnityPort, 'InputBufferSize', 900000);

fprintf('waiting for client');
fopen(UnityPort);
pause(1);
fprintf('client connected');

if(get(UnityPort,'BytesAvailable')>0)
    greeting = fgetl(UnityPort);
    greeting = strtrim(greeting);
end

KbName('UnifyKeyNames');  
escKEY = KbName('escape');
spacekey = KbName('space');
pkey = KbName('p');
Screen('Preference', 'SkipSyncTests', 2)

% prompt for UID
prompt = {'Enter subject ID:'}; %,'Enter configuration file name:'};

dlg_title = 'Subject ID';
% uuid = char(java.util.UUID.randomUUID());
% uuid = uuid(end-7:end);
uuid = 'Demo';
config_name = 'config.txt';
num_lines= 1;
def     = {uuid};%,config_name};
answer  = inputdlg(prompt,dlg_title,num_lines,def);

%if the user clicks 'cancel', 'answer' is empty. Quite the program.
if isempty(answer)
    return;
end
% end prompt

% Eyelink Setup
if EyelinkInit()~= 1 %
    return;
end

screenNumber=max(Screen('Screens'));
[window, wRect]=Screen('OpenWindow', screenNumber, 0,[],[],2);
Screen(window,'BlendFunction',GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
img = imread('cali_all.png'); % calibration image
texture1 = Screen('MakeTexture', window, img);

el=EyelinkInitDefaults(window);

Eyelink('Command', 'link_sample_data = LEFT,RIGHT,GAZE,AREA');

[v vs]=Eyelink('GetTrackerVersion');
edfFile=strcat(answer{1},'.edf');
Eyelink('Openfile', edfFile);
eye_used = el.RIGHT_EYE;

%Loop during eye tracking setup. Setup/Calibration image would be displayed
%spacebar quits this setup phase
while 1 % loop till error or space bar is pressed
        
        % check for keyboard press
        [keyIsDown,secs,keyCode] = KbCheck;
        
        % quit this phase when the spacebar is pressed
        if keyCode(spacekey)
            KbReleaseWait;
            break;
        end
        if keyCode(escKEY)
            sca
            Eyelink('ShutDown');
            return;
        end
        
        %display the setup/calibration image 'texture1'
        Screen('FillRect', window, el.backgroundcolour);
        Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
        Screen('Flip',  el.window);
end % setup loop

t = udp('localhost',64000,'LocalPort',64001, 'timeout', 1);
fopen(t);
allData = [];
sca

fprintf('Setting up screens');
screenNumber=max(Screen('Screens'));
[window, wRect]=Screen('OpenWindow', screenNumber, 0,[],32,2);
Screen(window,'BlendFunction',GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
Screen('GetFlipInterval', window)
img = imread('cali_all.png'); %numbers and shapes 'calibration' image
texture1 = Screen('MakeTexture', window, img); %display the numbers and shapes 'calibration' image.
EyelinkDoTrackerSetup(el); %brings EyeLink options display up (calibration, validation, etc.) ?
Eyelink('StartRecording');

%loop to display gaze data superimposed onto the 'setup/calibration
%numbers' image
while 1 % loop till error or space bar is pressed

    % Check recording status, stop display if error
    error=Eyelink('CheckRecording');
    if(error~=0)
        break;
    end

    % check for keyboard press, if spacebar was pressed stop display
    [keyIsDown,secs,keyCode] = KbCheck;
    if keyCode(spacekey)
        break;
    end

    % check for presence of a new sample update
    if Eyelink('NewFloatSampleAvailable') > 0

        % get the sample in the form of an event structure
        evt = Eyelink( 'NewestFloatSample');

            % if we do, get current gaze position from sample
            x = evt.gx(eye_used+1); % +1 as we're accessing MATLAB array
            y = evt.gy(eye_used+1);

            % do we have valid data and is the pupil visible?
            if x~=el.MISSING_DATA && y~=el.MISSING_DATA && evt.pa(eye_used+1)>0

                % if data is valid, draw a circle on the screen at current gaze position
                % using PsychToolbox's Screen function
                gazeRect=[ x-7 y-7 x+8 y+8];         
                x_str = num2str(x);
                y_str = num2str(y);     
                mos_pos = strcat(x_str, ',', y_str);         
                Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                Screen('FrameOval', window, el.foregroundcolour,gazeRect,6,6);       
                Screen('DrawText', window, mos_pos, x+50, y+50);
                Screen('Flip',  el.window, [], 1); % don't erase
                x_threshold = x+200;
            else
                % if data is invalid (e.g. during a blink), clear display
                Screen('FillRect', window, el.backgroundcolour);
                Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                Screen('Flip',  el.window);
            end
    end % if sample available
end % main loop

Eyelink('StopRecording');
sca


% eyelink works, let's set up the Optotrack stuff here.

%initialize optotrak if need be.
if iniOpto == 0
    optotrak_init
end

%setup collection parameters. 'nMarkersPerPort' is a 4 element array with
%'0' for the 4th element.
% OTCParamsHandler is creates a GUI prompt for the parameters
% realistically the values are probably known beforehand, maybe just set
% those as defaults so experimenter can "enter" through
%[nMarkers, smpRt, smpTime, nMarkersPerPort] = OTCParamsHandler;
nMArkers=1; smpRt=200; smpTime=2; nMarkersPerPort=[nMarkers 0 0 0];

%setup the strober specific information (number of markers per
%port)...NOTE, this function requires a value for a 4th port, but since the
%Certus only has 3, this 4th value is always 0
OptotrakSetStroberPortTable(nMarkersPerPort(1),nMarkersPerPort(2),...
    nMarkersPerPort(3),nMarkersPerPort(4));

%setup collection - note, these values are from Binstead code,
%which match those listed in the API.
nMarkers = sum(sum(nMarkersPerPort));
fMarkerFrequency = 2500.0;
nThreshold = 30;
nMinimumGain = 160;
nStreamData = 0;
fDutyCycle = 0.5;
fVoltage = 8.0;
fPreTriggerTime = 0.0;
nFlags = 0;

% setup.
OptotrakSetupCollection(nMarkers,smpRt,fMarkerFrequency,nThreshold,...
    nMinimumGain,nStreamData,fDutyCycle,fVoltage,smpTime,fPreTriggerTime,...
    nFlags);

%Raw data, Reorganized Data
A = {}; B = {};

%compute additional collection parameters
ISI = (1/smpRt); %inter-sample-interval (in seconds).
fTotPerTrial = smpTime/ISI; %the total number of sample frames to collect per trial

WaitSecs(1); %wait times recommended in API
OptotrakActivateMarkers();
WaitSecs(2); %wait times recommended in API

% optotrack set should be finished before the loop

run_experiment = true;
current_state = 1; % state machine behaviour
% 1 = waiting for unity (finger on homebutton)
% 2 = eyetracking, waiting for optotrack signal
% 3 = eyetracking + optotrack, waiting for end/exit
% for 2 and 3, when eyetracking sees that eye out of range, it will
% stop recording for all devices and send the 'Restart' signal to Unity

while(run_experiment)
    % check is server connection still there
    switch current_state
        case 1 % waiting for Unity to signal start
            fprintf(UnityPort,int2str(current_state));
            if(UnityPort.BytesAvailable)
                data = fscanf(UnityPort);
                data = strtrim(data);
                if contains(data, 'Eyelink')
                    % fetch the trial number also?
                    fprintf(UnityPort,'Fixation');
                    current_state = 2;
%                     Eyelink('StartRecording'); % start recording
                elseif contains(data,'Exit') % quit, maybe done experiment
                    fprintf(UnityPort,data);
                    run_experiment = false;
                else
                    % unexcepted behaviour? Tell Unity to start trial again
                    fprintf(UnityPort,'Restart');
                end
                fprintf(UnityPort,[data ": " int2str(current_state)]);
            end
        case 2 % eyetrack -- in a sense, MATLAB is in more control here
            if(eyeOnFixation(fixation_pos)) % eye is okay
                % check for message to start recording optotrack
                if(get(UnityPort,'BytesAvailable')>0)
                    data = fscanf(UnityPort);
                    data = strtrim(data);
                    if contains(data,'Optotrack')
% -------------- Start the optotrack recording here! -------------------- %
                        %start collecting data, the number of frames to collect was
                        %pre-specified by the function OPTOTRAKSETUPCOLLECTION.
%                         DataBufferStart();
                        fprintf(UnityPort,data);
                        current_state = 3;
                    elseif contains(data,'End') %Unity says to kill?!
%                         Eyelink('StopRecording');
                        fprintf(UnityPort,data);
                        current_state = 1;
                    elseif contains(data,'Exit') % quit, for whatever reason
%                         Eyelink('StopRecording');
                        fprintf(UnityPort,data);
                        current_state = 1; % this doesn't matter cause it won't loop again
                        run_experiment = false;
                    end
                end
            else % eye moved, let's start again
%                 Eyelink('StopRecording');
                fprintf(UnityPort,'Restart');
                current_state = 1;
            end
        case 3 % eyetracking and optotrack both on
            if(UnityPort.BytesAvailable > 0)
                data = fgetl(UnityPort);
                data = strtrim(data);
                if contains(data,'End')
% -------------------- stop optotrack here ---------------------------- %
%                     Eyelink('StopRecording');
                    fprintf(UnityPort,data);
                    current_state = 4;
                elseif contains(data,'Exit') % quit, for whatever reason
% -------------------- stop optotrack here ---------------------------- %
%                     Eyelink('StopRecording');
                    fprintf(UnityPort,data);
                    current_state = 4; % this doesn't matter cause it won't loop again
                    run_experiment = false;
                end
            end
            if(~eyeOnFixation(fixation_pos))
% -------------------- stop optotrack here ---------------------------- %
%                 Eyelink('StopRecording');
                current_state = 1;
            end
            
        case 4
% -------------------- spooling happens here -------------------------- %
%             Eyelink('StopRecording');
%             
%             %transfer data from the optotrak to the computer
%             while (puSpoolComplete == 0)
%                 % call C library function here. See PDF
%                 [puRealtimeData,puSpoolComplete,puSpoolStatus,pulFramesBuffered]=DataBufferWriteData(puRealtimeData,puSpoolComplete,puSpoolStatus,pulFramesBuffered);
%                 WaitSecs(.1);
%                 if puSpoolStatus ~= 0
%                     disp('Spooling Error');
%                     break;
%                 end 
%             end
            current_state = 1;
    end
    error=Eyelink('CheckRecording');
    if(error~=0)
        fprintf('something goes wrong');
    end
end

WaitSecs(0.1);
Eyelink('CloseFile');
% download data file
try
    fprintf('Receiving data file ''%s''\n', edfFile );
    status=Eyelink('ReceiveFile');
    if status > 0
        fprintf('ReceiveFile status %d\n', status);
    end
    if 2==exist(edfFile, 'file')
        fprintf('Data file ''%s'' can be found in ''%s''\n', edfFile, pwd );
    end
catch
    fprintf('Problem receiving data file ''%s''\n', edfFile );
end
Eyelink('ShutDown');
Screen('CloseAll');

fclose(t);
delete(t);


fclose(UnityPort);