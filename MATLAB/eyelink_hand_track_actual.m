clc
clear all;
commandwindow;
sca;

KbName('UnifyKeyNames');  
escKEY = KbName('escape');
spacekey=KbName('space');
pkey = KbName('p');
Screen('Preference', 'SkipSyncTests', 2)

num_trials = 4;        %change this if needed.          %change this if needed.                   %change this if needed.
x_threshold = 1200;    %change this if needed.          %change this if needed.                   %change this if needed.

% generate a UUID here since we're not using Unity
prompt = {'Enter subject ID:'};%,'Enter configuration file name:'};

dlg_title = 'Subject ID';
uuid = char(java.util.UUID.randomUUID());
uuid = uuid(end-7:end);
config_name = 'config.txt';
num_lines= 1;
def     = {uuid};%,config_name};
answer  = inputdlg(prompt,dlg_title,num_lines,def);

%if the user clicks 'cancel', 'answer' is empty. Quite the program.
if isempty(answer)
    return;
end

% open the config file
% config_table = readtable(fullfile(pwd,'config_charstream_dual',char(answer(2))));

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
Eyelink('NewFloatSampleAvailable')

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

%HideCursor;
t = udp('localhost',64000,'LocalPort',64001, 'timeout', 1);
fopen(t);
allData = [];
sca

%boolean loop checks
running = '0';
working = 1;
doing_trials = 1;

%trial and block counters
doing_trial = 1;
doing_block = 1;

% open a file for writing to track current state
% if ~exist(char(answer(2)));
% current_state_file = fopen(char(answer(2))+'_current_state','w');


while working
    doing_trial = 1;
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
    while 1 && working % loop till error or space bar is pressed
        
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
        % if there is, grab the latest, check validity, then display on
        % screen as a circle with coordinates
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
                    Screen('DrawText', window, mos_pos, x+50, y+50)  
                    Screen('Flip',  el.window, [], 1); % don't erase
                    x_threshold = x+200;
                else
                    % if data is invalid (e.g. during a blink), clear display
                    Screen('FillRect', window, el.backgroundcolour);
                    Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                    Screen('Flip',  el.window);
                end
        end % if sample available
    end % verification loop
     
    Eyelink('StopRecording');
    sca
    
    % Start the server for Unity Communication
    UnityPort = tcpip('localhost', 56789, 'NetworkRole', 'Server');
    set(UnityPort, 'InputBufferSize', 900000);
    fprintf('waiting for client');
    fopen(t); % This opens the connection
    fprintf("Connection established\n");
    
    %TRIAL LOOP
    while doing_trials && working
        
        %Check for error, user-determined quit (escape key), or an absense
        %of gaze data (which would be due to the eyelink not running).
        while running == '0' && working && doing_trials
            [keyIsDown,secs,keyCode] = KbCheck;
            
            % if spacebar was pressed stop display
            if keyCode(escKEY)
                working = 0;
                break
            end
            if keyCode(pkey)
                KbReleaseWait;
                doing_trials = 0;
            end
            
            %check to see if there is any more gaze data. If not, then the
            %eyelink is not running. Quit out of the trial loop.
            running = fscanf(t);
            if isempty(running)
                running = '0';
            end
        end
        
        %wait for input from Unity, process that data
        while(get(UnityPort, 'BytesAvailable') == 0)
            % wait until there's input from Unity
        end
        data = "";
        while(get(UnityPort, 'BytesAvailable') > 0 )
            UnityPort.BytesAvailable
            data = [data fread(UnityPort, UnityPort.BytesAvailable)];
        end
        
        
        tic
        Eyelink('Message', 'SYNCTIME');
        Eyelink('StartRecording');
        
        loop_counter = 0;
        
        %This will not run if: 'p'key was pressed, or if 'running'-'0', or
        %if 'working' is not 1 (so if Esc key was pressed)
        while running ~= '0' && working && doing_trials% loop till error or space bar is pressed
            loop_counter = loop_counter + 1;
            running = fscanf(t); %should be moved to the end of the while loop
            if isempty(running)  %this too
                running = '0';   %this too
            end                  %this too
        
            % Check recording status, stop display if error
            error=Eyelink('CheckRecording');
            if(error~=0)
                break;
            end
            % check for keyboard press
        
            % check for presence of a new sample update
            if Eyelink( 'NewFloatSampleAvailable') > 0
                % get the sample in the form of an event structure
                evt = Eyelink( 'NewestFloatSample');
            
                % if we do, get current gaze position from sample
                x = evt.gx(eye_used+1); % +1 as we're accessing MATLAB array
                y = evt.gy(eye_used+1);
                
                % do we have valid data and is the pupil visible?
                if x~=el.MISSING_DATA && y~=el.MISSING_DATA && evt.pa(eye_used+1)>0
                    
                    % if data is valid, draw a circle on the screen at current gaze position
                    % using PsychToolbox's Screen function
                    mouse.mouseMove(x, y);
                    
                    %if the current gaze x-position is less than the
                    %threshold, store '1', otherwise store '0'.
                    if x < x_threshold
                        fwrite(t, 49); %store '1'
                    else
                        fwrite(t, 48); % store '0' quits the loop next cycle.
                        
                    end
                    
                    %stores one additional cycle's worth of gaze data
                    %(beyond exceeding the x-threshold).
                    
                    to_store = [doing_block, doing_trial, str2double(running), x, y, toc];
                    allData = [allData; to_store];

                else
                    fwrite(t, 48); % store '0'
                end
            end % if sample available
        end % main loop
        
        Eyelink('StopRecording');
        doing_trial = doing_trial + 1;
        
    end %of trials
    
    doing_block = doing_block + 1;
    doing_trials = 1;

end

Eyelink('StopRecording');

WaitSecs(0.1);

% STEP 7
% finish up: stop recording eye-movements,
% close graphics window, close data file and shut down tracker

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

filename = strcat(answer{1},'.txt');
csvwrite(filename, allData)



%ShowCursor